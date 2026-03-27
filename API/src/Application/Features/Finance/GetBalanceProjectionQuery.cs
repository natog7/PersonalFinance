using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Features.Finance.Events;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Finance;

/// <summary>
/// Query to get balance projection for upcoming months.
/// </summary>
public record GetBalanceProjectionQuery : IRequest<ListResult<MonthlyProjection>>
{
    public int MonthCount { get; init; } = 12;
    public DateOnly StartDate { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);
	public List<Guid>? CategoryIds { get; init; } = null;
}

public record MonthlyProjection
(
	DateOnly Month,
	List<MoneyProjection> Balances,
	decimal RemainingPercentage
);

public record MoneyProjection
(
	decimal Amount,
	string? Currency,
	decimal RemainingPercentage
);

/// <summary>
/// Handler for GetBalanceProjectionQuery that calculates the balance projection.
/// Formula: Balance = Sum(IncomeTransactions) - Sum(ExpenseTransactions)
/// </summary>
public class GetBalanceProjectionQueryHandler : CommandHandler<GetBalanceProjectionQuery, ListResult<MonthlyProjection>, ITransactionRepository>
{
	private readonly IBalanceProjectionCacheService _cache;
	private readonly IBalanceProjectionMongoRepository _mongo;
	private readonly IBalanceProjectionProducer _producer;
	private static readonly string cacheKeyPrefix = "balance_projection";

	public GetBalanceProjectionQueryHandler(
		ITransactionRepository repository,
		ICurrentUserService userService,
		IBalanceProjectionCacheService cache,
		IBalanceProjectionMongoRepository mongo,
		IBalanceProjectionProducer producer)
		: base(repository, userService)
	{
		_cache = cache;
		_mongo = mongo;
		_producer = producer;
	}

	public override async Task<ListResult<MonthlyProjection>> Handle(GetBalanceProjectionQuery request, CancellationToken ct)
	{
		// Redis
		var cacheKey = BuildCacheKey(request);

		var cachedData = await _cache.GetAsync(cacheKey, ct);
		if (cachedData is not null)
			return cachedData;

		// MongoDB
		var persisted = await _mongo.GetAsync(cacheKey, ct);
		if (persisted is not null)
		{
			// Update Redis cache
			await _cache.SetAsync(cacheKey, persisted, ct);
			return persisted;
		}

		// RabbitMQ - Publish event to calculate projection asynchronously
		await _producer.PublishAsync(new CalculateBalanceProjectionEvent
		{
			MonthCount = request.MonthCount,
			StartDate = request.StartDate,
			CategoryIds = request.CategoryIds,
			CacheKey = cacheKey
		}, ct);

		return new ListResult<MonthlyProjection>();
	}

	private static string BuildCacheKey(GetBalanceProjectionQuery request)
	{
		var categoryPart = request.CategoryIds is { Count: > 0 }
			? string.Join("-", request.CategoryIds.OrderBy(x => x))
			: "all";
		return $"{cacheKeyPrefix}:{request.StartDate:yyyy-MM}:{request.MonthCount}:{categoryPart}";
	}
}

public class GetBalanceProjectionQueryValidator : AbstractValidator<GetBalanceProjectionQuery>
{
	public GetBalanceProjectionQueryValidator()
	{
		RuleFor(x => x.MonthCount).IsInclusiveBetween(1, 12);
	}
}
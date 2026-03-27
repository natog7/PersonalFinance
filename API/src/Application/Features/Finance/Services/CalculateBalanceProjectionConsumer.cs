using MassTransit;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Domain.Enums;

namespace PersonalFinanceAPI.Application.Features.Finance.Services;

public class CalculateBalanceProjectionConsumer : IConsumer<CalculateBalanceProjectionEvent>
{
	private readonly ITransactionRepository _repository;
	private readonly IBalanceProjectionCacheService _cache;
	private readonly IBalanceProjectionMongoRepository _mongo;

	public CalculateBalanceProjectionConsumer(
		ITransactionRepository repository,
		IBalanceProjectionCacheService cache,
		IBalanceProjectionMongoRepository mongo)
	{
		_repository = repository;
		_cache = cache;
		_mongo = mongo;
	}

	public async Task Consume(ConsumeContext<CalculateBalanceProjectionEvent> context)
	{
		var msg = context.Message;
		var ct = context.CancellationToken;

		// Set to first day of the month for consistency
		var startDate = new DateOnly(msg.StartDate.Year, msg.StartDate.Month, 1);
		var endDate = startDate.AddMonths(msg.MonthCount).AddDays(-1);

		var monthlySums = await _repository.GetMonthlySumsAsync(startDate, endDate, msg.CategoryIds, ct);
		var currencies = monthlySums.Keys.Select(k => k.Currency).Distinct().ToList();

		var projections = new List<MonthlyProjection>(msg.MonthCount);

		for (int i = 0; i < msg.MonthCount; i++)
		{
			var projectionDate = startDate.AddMonths(i);

			var monthlyBalances = currencies.Select(currency =>
			{
				var income = monthlySums.GetValueOrDefault((projectionDate.Year, projectionDate.Month, currency, TransactionType.Income));
				var expenses = monthlySums.GetValueOrDefault((projectionDate.Year, projectionDate.Month, currency, TransactionType.Expense));

				var balance = income - expenses;
				var remaining = income > 0 ? (balance / income) * 100 : 0;

				return new MoneyProjection(balance, currency, Math.Max(0, remaining));
			}).ToList();

			var avgRemaining = monthlyBalances.Count > 0
				? monthlyBalances.Average(x => x.RemainingPercentage)
				: 0;

			projections.Add(new MonthlyProjection(projectionDate, monthlyBalances, avgRemaining));
		}

		var result = new ListResult<MonthlyProjection> { Items = projections };

		// Save on Redis and MongoDB
		await Task.WhenAll(
			_mongo.SaveAsync(msg.CacheKey, result, ct),
			_cache.SetAsync(msg.CacheKey, result, ct)
		);

		//await context.Publish(new BalanceProjectionCalculatedEvent
		//{
		//	CorrelationId = msg.CorrelationId,
		//	CacheKey = msg.CacheKey,
		//	Result = result
		//}, ct);
	}
}

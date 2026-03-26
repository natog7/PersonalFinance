using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Features.Transactions.Queries;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

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
	public GetBalanceProjectionQueryHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<ListResult<MonthlyProjection>> Handle(GetBalanceProjectionQuery request, CancellationToken ct)
	{
		// Set to first day of the month for consistency
		var startDate = new DateOnly(request.StartDate.Year, request.StartDate.Month, 1);
		var endDate = startDate.AddMonths(request.MonthCount).AddDays(-1);

		var monthlySums = await _repository.GetMonthlySumsAsync(
		startDate, endDate, request.CategoryIds, ct);

		var currencies = monthlySums.Keys
			.Select(k => k.Currency)
			.Distinct()
			.ToList();

		var projections = new List<MonthlyProjection>(request.MonthCount);

		for (int i = 0; i < request.MonthCount; i++)
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

		return new ListResult<MonthlyProjection> { Items = projections };
	}
}

public class GetBalanceProjectionQueryValidator : AbstractValidator<GetBalanceProjectionQuery>
{
	public GetBalanceProjectionQueryValidator()
	{
		RuleFor(x => x.MonthCount).IsInclusiveBetween(1, 12);
	}
}
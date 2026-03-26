using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions.Queries;

/// <summary>
/// Query to get balance projection for upcoming months.
/// </summary>
public record GetBalanceProjectionQuery : IRequest<ListResult<MonthlyProjection>>
{
    public int MonthCount { get; init; } = 12;
    public DateOnly StartDate { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);
}

public record MonthlyProjection(
	int Year,
	int Month,
	Money ProjectedBalance,
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
		if (request.MonthCount <= 0)
			throw new ArgumentException("Month count must be greater than zero.", nameof(request.MonthCount));

		var result = new ListResult<MonthlyProjection>();

		// Calculate monthly projections
		for (int i = 0; i < request.MonthCount; i++)
		{
			var projectionDate = request.StartDate.AddMonths(i);
			var year = projectionDate.Year;
			var month = projectionDate.Month;

			// Get transactions for this month
			var monthTransactions = await _repository.GetFilterAsync(new GetTransactionsQuery()
			{
				Date = DateOnlyPeriod.Create(new DateOnly(year, month, 1), new DateOnly(year, month, DateTime.DaysInMonth(year, month)))
			}, ct);

			// Calculate balance: Income - Expense
			var income = monthTransactions
				.Where(t => t.Type == TransactionType.Income)
				.Sum(t => t.Amount.Amount);

			var expenses = monthTransactions
				.Where(t => t.Type == TransactionType.Expense)
				.Sum(t => t.Amount.Amount);

			//var monthlyBalance = income - expenses;
			//var projectedBalance = Money.Create(monthlyBalance, budget.LimitAmount.Currency);
			//var remainingAmount = budget.LimitAmount.Amount - expenses;
			//var remainingPercentage = budget.LimitAmount.Amount > 0
			//    ? (remainingAmount / budget.LimitAmount.Amount) * 100
			//    : 0;

			//result.Projections.Add(new MonthlyProjection
			//{
			//    Year = year,
			//    Month = month,
			//    ProjectedBalance = projectedBalance,
			//    BudgetLimit = budget.LimitAmount,
			//    RemainingPercentage = Math.Max(0, remainingPercentage)
			//});
		}

		return result;
	}
}

public class GetBalanceProjectionQueryValidator : AbstractValidator<GetBalanceProjectionQuery>
{
	public GetBalanceProjectionQueryValidator()
	{
		RuleFor(x => x.MonthCount).IsInclusiveBetween(1, 12);
	}
}
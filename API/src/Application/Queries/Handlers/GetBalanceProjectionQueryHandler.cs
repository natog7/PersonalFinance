using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Enums;

namespace PersonalFinanceAPI.Application.Queries.Handlers;

/// <summary>
/// Handler for GetBalanceProjectionQuery that calculates the balance projection.
/// Formula: Balance = Sum(IncomeTransactions) - Sum(ExpenseTransactions)
/// </summary>
public class GetBalanceProjectionQueryHandler : IRequestHandler<GetBalanceProjectionQuery, GetBalanceProjectionResult>
{
    private readonly ITransactionRepository _repository;

    public GetBalanceProjectionQueryHandler(ITransactionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetBalanceProjectionResult> Handle(
        GetBalanceProjectionQuery request,
        CancellationToken cancellationToken)
    {
        if (request.MonthCount <= 0)
            throw new ArgumentException("Month count must be greater than zero.", nameof(request.MonthCount));

        var result = new GetBalanceProjectionResult { CategoryId = request.CategoryId };

        // Get budget information for the category
        //var budget = await _dbContext.Budgets
        //    .FirstOrDefaultAsync(b => b.CategoryId == request.CategoryId && b.IsActive, cancellationToken);

        //if (budget is null)
        //    return result; // Return empty projections if no active budget

        // Get all confirmed transactions for the category
        var transactions = await _repository.GetByCategoryAsync(request.CategoryId, cancellationToken);

        // Calculate monthly projections
        for (int i = 0; i < request.MonthCount; i++)
        {
            var projectionDate = request.StartDate.AddMonths(i);
            var year = projectionDate.Year;
            var month = projectionDate.Month;

            // Get transactions for this month
            var monthTransactions = transactions
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .ToList();

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

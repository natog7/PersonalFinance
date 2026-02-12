using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Queries;

/// <summary>
/// Query to get balance projection for upcoming months.
/// </summary>
public class GetBalanceProjectionQuery : IRequest<GetBalanceProjectionResult>
{
    public Guid CategoryId { get; set; }
    public int MonthCount { get; set; } = 12;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}

public class GetBalanceProjectionResult
{
    public Guid CategoryId { get; set; }
    public List<MonthlyProjection> Projections { get; set; } = new();
}

public class MonthlyProjection
{
    public int Year { get; set; }
    public int Month { get; set; }
    public Money ProjectedBalance { get; set; } = null!;
    public Money BudgetLimit { get; set; } = null!;
    public decimal RemainingPercentage { get; set; }
}

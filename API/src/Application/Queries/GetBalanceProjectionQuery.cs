using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Queries;

/// <summary>
/// Query to get balance projection for upcoming months.
/// </summary>
public class GetBalanceProjectionQuery : IRequest<GetBalanceProjectionResult>
{
    public int MonthCount { get; set; } = 12;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}

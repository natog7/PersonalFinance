using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Queries;

public class MonthlyProjection
{
	public int Year { get; set; }
	public int Month { get; set; }
	public Money ProjectedBalance { get; set; } = null!;
	public decimal RemainingPercentage { get; set; }
}
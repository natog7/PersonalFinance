namespace PersonalFinanceAPI.Application.Queries;

public class GetBalanceProjectionResult
{
	public List<MonthlyProjection> Projections { get; set; } = new();
}
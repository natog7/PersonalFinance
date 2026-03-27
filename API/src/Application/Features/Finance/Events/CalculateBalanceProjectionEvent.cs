namespace PersonalFinanceAPI.Application.Features.Finance.Events;

public record CalculateBalanceProjectionEvent
{
	public Guid CorrelationId { get; init; } = Guid.NewGuid();
	public int MonthCount { get; init; }
	public DateOnly StartDate { get; init; }
	public List<Guid>? CategoryIds { get; init; }
	public string CacheKey { get; init; } = default!;
}

using PersonalFinanceAPI.Application.Features.Shared.Events;

namespace PersonalFinanceAPI.Application.Features.Finance.Events;

public record CalculateBalanceProjectionEvent : CorrelatedEvent
{
	public string CacheKey { get; init; } = default!;
	public int MonthCount { get; init; }
	public DateOnly StartDate { get; init; }
	public List<Guid>? CategoryIds { get; init; }
}

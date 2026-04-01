namespace PersonalFinanceAPI.Application.Features.Shared.Events;

public record CorrelatedEvent
{
	public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
using PersonalFinanceAPI.Application.Features.Shared.Events;

namespace PersonalFinanceAPI.Application.Services;

public interface IEventProducer<T> where T : CorrelatedEvent
{
	Task PublishAsync(T ev, CancellationToken ct = default);
}

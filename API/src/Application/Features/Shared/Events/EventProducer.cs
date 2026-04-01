using MassTransit;
using PersonalFinanceAPI.Application.Services;

namespace PersonalFinanceAPI.Application.Features.Shared.Events;

public class EventProducer<T> : IEventProducer<T> where T : CorrelatedEvent
{
	private readonly IPublishEndpoint _publisher;
	public EventProducer(IPublishEndpoint publisher) => _publisher = publisher;

	public Task PublishAsync(T ev, CancellationToken ct = default)
		=> _publisher.Publish(ev, ct);
}

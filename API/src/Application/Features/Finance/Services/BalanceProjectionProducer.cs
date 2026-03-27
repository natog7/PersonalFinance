using MassTransit;
using PersonalFinanceAPI.Application.Features.Finance.Events;
using PersonalFinanceAPI.Application.Services;

namespace PersonalFinanceAPI.Application.Features.Finance.Services;

public class BalanceProjectionProducer : IBalanceProjectionProducer
{
	private readonly IPublishEndpoint _publishEndpoint;
	public BalanceProjectionProducer(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

	public Task PublishAsync(CalculateBalanceProjectionEvent ev, CancellationToken ct = default)
		=> _publishEndpoint.Publish(ev, ct);
}

using PersonalFinanceAPI.Application.Features.Finance.Events;

namespace PersonalFinanceAPI.Application.Services;

public interface IBalanceProjectionProducer
{
	Task PublishAsync(CalculateBalanceProjectionEvent ev, CancellationToken ct = default);
}

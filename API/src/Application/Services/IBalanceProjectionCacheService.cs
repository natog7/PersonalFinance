using PersonalFinanceAPI.Application.Features.Finance;

namespace PersonalFinanceAPI.Application.Services;

public interface IBalanceProjectionCacheService
{
	Task<ListResult<MonthlyProjection>?> GetAsync(string key, CancellationToken ct = default);
	Task SetAsync(string key, ListResult<MonthlyProjection> value, CancellationToken ct = default);
}

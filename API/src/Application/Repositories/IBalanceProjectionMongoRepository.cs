using PersonalFinanceAPI.Application.Features.Finance;

namespace PersonalFinanceAPI.Application.Repositories;

public interface IBalanceProjectionMongoRepository
{
	Task<ListResult<MonthlyProjection>?> GetAsync(string key, CancellationToken ct = default);
	Task SaveAsync(string key, ListResult<MonthlyProjection> value, CancellationToken ct = default);
}
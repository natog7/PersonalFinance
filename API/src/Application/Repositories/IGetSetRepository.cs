namespace PersonalFinanceAPI.Application.Repositories;

public interface IGetSetRepository<T>
{
	Task<T?> GetAsync(string key, CancellationToken ct = default);
	Task SaveAsync(string key, T value, CancellationToken ct = default);
}
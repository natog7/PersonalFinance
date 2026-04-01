namespace PersonalFinanceAPI.Application.Services;

public interface ICacheService<T>
{
	Task<T?> GetAsync(string key, CancellationToken ct = default);
	Task SetAsync(string key, T value, CancellationToken ct = default);
}

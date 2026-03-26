
namespace PersonalFinanceAPI.Application.Repositories;

public interface IRepository<T, TId>
	where T : class
	where TId : struct, IEquatable<TId>
{
	Task<T?> GetByIdAsync(TId id, CancellationToken ct = default);
	Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
	Task AddAsync(T entity, CancellationToken ct = default);
	Task UpdateAsync(T entity, CancellationToken ct = default);
	Task DeleteAsync(TId id, CancellationToken ct = default);
}

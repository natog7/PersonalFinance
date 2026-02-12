
namespace PersonalFinanceAPI.Application.Repositories;

public interface IRepository<T, TId>
	where T : class
	where TId : struct, IEquatable<TId>
{
	Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
	Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
	Task AddAsync(T entity, CancellationToken cancellationToken = default);
	Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
	Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
}

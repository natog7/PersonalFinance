using PersonalFinanceAPI.Domain.Entities;

namespace PersonalFinanceAPI.Application.Repositories;

/// <summary>
/// Repository interface for Transaction aggregate root.
/// Defines contract for data access operations on transactions.
/// </summary>
public interface ITransactionRepository : IRepository<Transaction, Guid>
{
    Task<List<Transaction>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<List<Transaction>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdempotencyHashAsync(string hash, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    IQueryable<Transaction> GetQueryable();
}

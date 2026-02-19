using PersonalFinanceAPI.Application.Queries;
using PersonalFinanceAPI.Domain.Entities;

namespace PersonalFinanceAPI.Application.Repositories;

/// <summary>
/// Repository interface for Transaction aggregate root.
/// Defines contract for data access operations on transactions.
/// </summary>
public interface ITransactionRepository : IRepository<Transaction, Guid>
{
    Task<List<Transaction>> GetFilterAsync(GetTransactionsQuery filters, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    IQueryable<Transaction> GetQueryable();
}

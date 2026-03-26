using PersonalFinanceAPI.Application.Features.Transactions.Queries;
using PersonalFinanceAPI.Domain.Entities;

namespace PersonalFinanceAPI.Application.Repositories;

/// <summary>
/// Repository interface for Transaction aggregate root.
/// Defines contract for data access operations on transactions.
/// </summary>
public interface ITransactionRepository : IRepository<Transaction, Guid>
{
    Task<List<Transaction>> GetFilterAsync(GetTransactionsQuery filters, CancellationToken ct = default);
    Task<int> GetCountAsync(CancellationToken ct = default);
    IQueryable<Transaction> GetQueryable();
}

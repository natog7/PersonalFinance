using PersonalFinanceAPI.Application.Features.Transactions.Queries;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;
using System.Linq.Expressions;

namespace PersonalFinanceAPI.Application.Repositories;

/// <summary>
/// Repository interface for Transaction aggregate root.
/// Defines contract for data access operations on transactions.
/// </summary>
public interface ITransactionRepository : IRepository<Transaction, Guid>
{
    Task<List<Transaction>> GetFilterAsync(GetTransactionsQuery filters, CancellationToken ct = default);
	Task<Dictionary<(int Year, int Month, string Currency, TransactionType Type), decimal>>
        GetMonthlySumsAsync(DateOnly start, DateOnly end, List<Guid>? categoryIds, CancellationToken ct);
	Task<int> GetCountAsync(CancellationToken ct = default);
    Task<List<T>> GetDistinct<T>(Expression<Func<Transaction, T>> selector, CancellationToken ct = default);
    IQueryable<Transaction> GetQueryable();
}

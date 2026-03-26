using PersonalFinanceAPI.Application.Features.Transactions.Queries;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Transaction aggregate root.
/// Provides CRUD operations and specialized queries.
/// </summary>
public class TransactionRepository : BaseRepository, ITransactionRepository
{
	public TransactionRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
	}

	public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken ct)
	{
		return await _dbContext.Transactions.AsNoTracking().OrderByDescending(t => t.Date).ToListAsync(ct);
	}

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        _dbContext.Transactions.Update(transaction);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var transaction = await GetByIdAsync(id, ct);
        if (transaction is not null)
        {
            _dbContext.Transactions.Remove(transaction);
            await _dbContext.SaveChangesAsync(ct);
        }
	}

	public async Task<List<Transaction>> GetFilterAsync(GetTransactionsQuery filters, CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(filters);

		var query = _dbContext.Transactions.AsNoTracking();

		if (!string.IsNullOrWhiteSpace(filters.Title))
		{
			query = query.Where(t => t.Title.Contains(filters.Title));
		}
		if (filters.Date is not null)
		{
			query = query.Where(t =>
				t.Date >= filters.Date.Start &&
				t.Date <= (filters.Date.End ?? filters.Date.Start));
		}
		if(filters.Type is not null)
		{
			query = query.Where(t => t.Type == filters.Type);
		}
		if (!string.IsNullOrWhiteSpace(filters.Currency))
		{
			query = query.Where(t => t.Amount.Currency == filters.Currency);
		}
		if (filters.CategoryIds is not null && filters.CategoryIds.Count > 0)
		{
			query = query.Where(t => filters.CategoryIds.Contains(t.CategoryId));
		}

		return await query.Include(t => t.Category).OrderByDescending(t => t.Date).ToListAsync(ct);
	}

	public async Task<Dictionary<(int Year, int Month, string Currency, TransactionType Type), decimal>>
		GetMonthlySumsAsync(DateOnly start, DateOnly end, List<Guid>? categoryIds, CancellationToken ct)
	{
		var query = _dbContext.Transactions.AsNoTracking().Where(t => t.Date >= start && t.Date <= end);

		if (categoryIds is not null && categoryIds.Count > 0)
			query = query.Where(t => categoryIds.Contains(t.CategoryId));

		return await query
			.GroupBy(t => new { t.Date.Year, t.Date.Month, t.Amount.Currency, t.Type })
			.Select(g => new
			{
				g.Key.Year,
				g.Key.Month,
				g.Key.Currency,
				g.Key.Type,
				Total = g.Sum(t => t.Amount.Amount)
			})
			.ToDictionaryAsync(
				x => (x.Year, x.Month, x.Currency, x.Type),
				x => x.Total,
				ct);
	}

	public async Task<int> GetCountAsync(CancellationToken ct = default)
    {
        return await _dbContext.Transactions.CountAsync(ct);
	}

	public async Task<List<T>> GetDistinct<T>(Expression<Func<Transaction, T>> selector, CancellationToken ct = default)
	{
		return await _dbContext.Transactions
			.AsNoTracking()
			.Select(selector)
			.Distinct()
			.ToListAsync(ct);
	}

	public IQueryable<Transaction> GetQueryable()
    {
        return _dbContext.Transactions.Include(t => t.Category).AsQueryable();
	}
}

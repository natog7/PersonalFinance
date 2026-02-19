using PersonalFinanceAPI.Infrastructure.Persistence;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Queries;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Transaction aggregate root.
/// Provides CRUD operations and specialized queries.
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
	}

	public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.Transactions.Update(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await GetByIdAsync(id, cancellationToken);
        if (transaction is not null)
        {
            _dbContext.Transactions.Remove(transaction);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
	}

	public async Task<List<Transaction>> GetFilterAsync(GetTransactionsQuery filters, CancellationToken cancellationToken = default)
	{
		var query = _dbContext.Transactions.AsNoTracking().AsQueryable();
		if (!string.IsNullOrEmpty(filters.Title))
		{
			query = query.Where(t => t.Title.Contains(filters.Title));
		}
		if (filters.Date is not null)
		{
			if(filters.Date.End is not null)
			{
				query = query.Where(t => t.Date >= filters.Date.Start && t.Date <= filters.Date.End);
			}
			else
			{
				query = query.Where(t => t.Date == filters.Date.Start);
			}
		}
		if(filters.Type is not null)
		{
			query = query.Where(t => t.Type == filters.Type);
		}
		if(filters.CategoryIds is not null && filters.CategoryIds.Any())
		{
			query = query.Where(t => filters.CategoryIds.Contains(t.CategoryId));
		}
		return await query.Include(t => t.Category).OrderByDescending(t => t.Date).ToListAsync(cancellationToken);
	}

	public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Transactions.CountAsync(cancellationToken);
    }

    public IQueryable<Transaction> GetQueryable()
    {
        return _dbContext.Transactions.Include(t => t.Category).AsQueryable();
	}
}

using PersonalFinanceAPI.Infrastructure.Persistence;
using PersonalFinanceAPI.Application.Repositories;

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

	public async Task<List<Transaction>> GetByCategoryAsync(
		Guid categoryId,
		CancellationToken cancellationToken = default)
	{
		return await _dbContext.Transactions
			.AsNoTracking()
			.Where(t => t.CategoryId == categoryId)
			.OrderByDescending(t => t.Date)
			.ToListAsync(cancellationToken);
	}

	public async Task<List<Transaction>> GetByDateRangeAsync(
		DateOnly startDate,
		DateOnly endDate,
		CancellationToken cancellationToken = default)
	{
		return await _dbContext.Transactions
			.AsNoTracking()
			.Where(t => t.Date >= startDate && t.Date <= endDate)
			.OrderByDescending(t => t.Date)
			.ToListAsync(cancellationToken);
	}

	public async Task<Transaction?> GetByIdempotencyHashAsync(
		string hash,
		CancellationToken cancellationToken = default)
	{
		return await _dbContext.Transactions
			.AsNoTracking()
			.FirstOrDefaultAsync(t => t.IdempotencyHash == hash, cancellationToken);
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

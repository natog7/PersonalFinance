using PersonalFinanceAPI.Application.Features.Finance;
using PersonalFinanceAPI.Application.Features.Transactions.Queries;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;
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
		if (filters.Type is not null)
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

	public async Task<List<CategoryTransactionSumDto>> GetTransactionsByCategoryAsync(
		DateOnlyPeriod date,
		TransactionType? type,
		List<Guid>? categoryIds,
		CancellationToken ct)
	{
		var query = _dbContext.Transactions
			.AsNoTracking();

		query = query.Where(t => t.Date >= date.Start && t.Date <= (date.End ?? date.Start));

		if (type.HasValue)
		{
			query = query.Where(t => t.Type == type.Value);
		}

		if (categoryIds is { Count: > 0 })
		{
			query = query.Where(t => categoryIds.Contains(t.CategoryId));
		}

		return await query
			.GroupBy(t => new
			{
				t.CategoryId,
				CategoryName = t.Category.Name,
				t.Amount.Currency
			})
			.Select(g => new CategoryTransactionSumDto
			{
				CategoryId = g.Key.CategoryId,
				CategoryName = g.Key.CategoryName,
				Currency = g.Key.Currency,
				TotalAmount = g.Sum(t => t.Amount.Amount)
			})
			.ToListAsync(ct);
	}

	public async Task<List<BalanceByMonthDto>> GetBalanceByMonthAsync(
		DateOnlyPeriod date,
		List<Guid>? categoryIds,
		CancellationToken ct)
	{
		var startDate = date.Start.AddMonths(-1);
		var queryStart = new DateOnly(startDate.Year, startDate.Month, 1);
		var queryEnd = date.End ?? date.Start;

		var query = _dbContext.Transactions
			.AsNoTracking();

		query = query.Where(t => t.Date >= queryStart && t.Date <= queryEnd);

		if (categoryIds is { Count: > 0 })
		{
			query = query.Where(t => categoryIds.Contains(t.CategoryId));
		}

		var grouped = await query
			.GroupBy(t => new
			{
				t.Date.Year,
				t.Date.Month,
				t.Amount.Currency
			})
			.Select(g => new
			{
				g.Key.Year,
				g.Key.Month,
				g.Key.Currency,
				Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount.Amount),
				Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount.Amount)
			})
			.ToListAsync(ct);

		var allMonths = grouped.Select(x => new BalanceByMonthDto
		{
			Month = new DateOnly(x.Year, x.Month, 1),
			Currency = x.Currency,
			TotalIncome = x.Income,
			TotalExpense = x.Expense,
			Total = x.Income - x.Expense
		})
		.OrderBy(x => x.Month)
		.ToList();

		var result = new List<BalanceByMonthDto>();
		var requestedStart = new DateOnly(date.Start.Year, date.Start.Month, 1);

		foreach (var item in allMonths)
		{
			if (item.Month < requestedStart) continue;

			var prevMonth = item.Month.AddMonths(-1);
			var prevData = allMonths.FirstOrDefault(x => x.Month == prevMonth && x.Currency == item.Currency);

			if (prevData != null)
			{
				item.TotalIncomeGrowthPercentage = CalculateGrowth(prevData.TotalIncome, item.TotalIncome);
				item.TotalExpenseGrowthPercentage = CalculateGrowth(prevData.TotalExpense, item.TotalExpense);
				item.TotalGrowthPercentage = CalculateGrowth(prevData.Total, item.Total);
			}

			result.Add(item);
		}

		return result;
	}

	private static decimal CalculateGrowth(decimal previous, decimal current)
	{
		if (previous == 0) return current == 0 ? 0 : 100;
		return (current - previous) / Math.Abs(previous) * 100;
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

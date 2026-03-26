using PersonalFinanceAPI.Application.Features.Categories.Queries;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Infrastructure.Persistence;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
	public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

	public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
	{
		return await _dbContext.Categories
			.AsNoTracking()
			.FirstOrDefaultAsync(t => t.Id == id, ct);
	}

	public async Task AddAsync(Category entity, CancellationToken ct = default)
	{
		_dbContext.Categories.Add(entity);
		await _dbContext.SaveChangesAsync(ct);
	}

	public async Task UpdateAsync(Category entity, CancellationToken ct = default)
	{
		_dbContext.Categories.Update(entity);
		await _dbContext.SaveChangesAsync(ct);
	}

	public async Task DeleteAsync(Guid id, CancellationToken ct = default)
	{
		var category = await GetByIdAsync(id, ct);
		if (category is not null)
		{
			_dbContext.Categories.Remove(category);
			await _dbContext.SaveChangesAsync(ct);
		}
	}

	public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
	{
		return await _dbContext.Categories.AsNoTracking().OrderByDescending(t => t.Name).ToListAsync(ct);
	}

	public async Task<List<Category>> GetFilterAsync(GetCategoriesQuery filters, CancellationToken ct = default)
	{
		var query = _dbContext.Categories.AsNoTracking();
		if (!string.IsNullOrEmpty(filters.Name))
		{
			query = query.Where(t => t.Name.Contains(filters.Name));
		}
		if (!string.IsNullOrEmpty(filters.Description))
		{
			query = query.Where(t => !string.IsNullOrEmpty(t.Description) && t.Description.Contains(filters.Description));
		}
		if (filters.ParentCategoryId is not null)
		{
			query = query.Where(t => t.ParentCategoryId == filters.ParentCategoryId);
		}
		if (filters.IsActive.HasValue)
		{
			query = query.Where(t => t.IsActive == filters.IsActive.Value);
		}
		return await query.OrderByDescending(t => t.Name).ToListAsync(ct);
	}

	public async Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken ct = default)
	{
		return await _dbContext.Transactions.AsNoTracking().Where(t => t.CategoryId == categoryId).AnyAsync(ct);
	}
}

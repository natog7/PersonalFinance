using PersonalFinanceAPI.Application.Features.Categories;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Infrastructure.Persistence;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
	public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

	public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Categories
			.AsNoTracking()
			.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
	}

	public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
	{
		_dbContext.Categories.Add(entity);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
	{
		_dbContext.Categories.Update(entity);
		await _dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var category = await GetByIdAsync(id, cancellationToken);
		if (category is not null)
		{
			_dbContext.Categories.Remove(category);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}

	public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _dbContext.Categories.AsNoTracking().OrderByDescending(t => t.Name).ToListAsync(cancellationToken);
	}

	public async Task<List<Category>> GetFilterAsync(GetCategoriesQuery filters, CancellationToken cancellationToken = default)
	{
		var query = _dbContext.Categories.AsNoTracking().AsQueryable();
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
		return await query.OrderByDescending(t => t.Name).ToListAsync(cancellationToken);
	}

	public async Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default)
	{
		return await _dbContext.Transactions.AsNoTracking().Where(t => t.CategoryId == categoryId).AnyAsync(cancellationToken);
	}
}

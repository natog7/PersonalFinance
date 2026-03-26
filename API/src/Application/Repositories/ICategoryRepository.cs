using PersonalFinanceAPI.Application.Features.Categories.Queries;
using PersonalFinanceAPI.Domain.Entities;

namespace PersonalFinanceAPI.Application.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
	Task<List<Category>> GetFilterAsync(GetCategoriesQuery filters, CancellationToken ct = default);
	Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken ct = default);
}

using PersonalFinanceAPI.Application.Features.Categories;
using PersonalFinanceAPI.Domain.Entities;

namespace PersonalFinanceAPI.Application.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
	Task<List<Category>> GetFilterAsync(GetCategoriesQuery filters, CancellationToken cancellationToken = default);
	Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default);
}

using PersonalFinanceAPI.Application.Queries;
using PersonalFinanceAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
	//Task<List<Category>> GetFilterAsync(GetCategoriesQuery filters, CancellationToken cancellationToken = default);
}

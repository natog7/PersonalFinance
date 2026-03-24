using PersonalFinanceAPI.Application.Features.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class GetCategoriesResult
{
	public List<CategoryDto> Categories { get; set; } = new();
}

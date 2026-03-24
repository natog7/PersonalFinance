using PersonalFinanceAPI.Application.Features.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class GetCategoryQuery : IRequest<CategoryDto?>
{
	public Guid Id { get; set; }
}

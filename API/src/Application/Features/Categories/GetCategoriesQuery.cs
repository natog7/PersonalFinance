using PersonalFinanceAPI.Application.Queries;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class GetCategoriesQuery : IRequest<GetCategoriesResult>
{
	public string? Name { get; set; }
	public string? Description { get; set; }
	public Guid? ParentCategoryId { get; private set; }
	public bool? IsActive { get; private set; }
}

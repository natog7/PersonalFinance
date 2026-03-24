using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record CreateCategoryCommand : IRequest<IdDto<Guid>>
{
	public string Name { get; private set; } = string.Empty;
	public string? Description { get; private set; }
	public string Color { get; private set; } = "#000000";
	public Guid? ParentCategoryId { get; private set; }
}

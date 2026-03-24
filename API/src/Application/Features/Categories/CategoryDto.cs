using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record CategoryDto : IdDto<Guid>
{
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string Color { get; set; } = "#000000";
	public Guid? ParentCategoryId { get; set; }
	public bool IsActive { get; set; }
}

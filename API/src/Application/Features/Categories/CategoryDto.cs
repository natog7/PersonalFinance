using PersonalFinanceAPI.Domain.Entities.Interfaces;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record CategoryDto : IdDto<Guid>, ICategoryFields
{
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string Color { get; set; } = "#000000";
	public string? Icon { get; set; }
	public Guid? ParentCategoryId { get; set; }
	public bool IsActive { get; set; }
}

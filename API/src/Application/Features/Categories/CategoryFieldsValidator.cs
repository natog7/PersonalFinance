using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Domain.Entities.Interfaces;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class CategoryFieldsValidator<T> : AbstractValidator<T> where T : ICategoryFields
{
	public CategoryFieldsValidator()
	{
		RuleFor(x => x.Name).NotEmptyMaxLength(64);
		RuleFor(x => x.Description).NotEmptyMaxLength(256);
		//.When(x => !string.IsNullOrWhiteSpace(x.Description));
		RuleFor(x => x.Color).IsHexColor();
	}
}

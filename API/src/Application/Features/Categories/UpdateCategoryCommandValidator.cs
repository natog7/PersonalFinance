using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class UpdateCategoryValidator<T> : AbstractValidator<T> where T : UpdateCategoryCommand
{
	public UpdateCategoryValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID cannot be empty.");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Name is required.")
			.MaximumLength(128).WithMessage("Name cannot exceed 128 characters.");

		RuleFor(x => x.Description)
			.MaximumLength(512).WithMessage("Description cannot exceed 512 characters.")
			.When(x => !string.IsNullOrWhiteSpace(x.Description));

		RuleFor(x => x.Color)
			.Matches(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")
			.WithMessage("Color must be a valid hexadecimal color (e.g. #FFF or #FFFFFF).")
			.When(x => !string.IsNullOrWhiteSpace(x.Color));
	}
}
using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record UpdateCategoryCommand(Guid Id, string? Name, string? Description, string? Color, Guid? ParentCategoryId, bool? IsActive) : IRequest<CategoryDto>;

public class UpdateCategoryCommandHandler : CommandHandler<UpdateCategoryCommand, CategoryDto, ICategoryRepository>
{
	public UpdateCategoryCommandHandler(ICategoryRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		var category = await _repository.GetByIdAsync(request.Id, ct)
			?? throw new Exception("Not found.");

		category.Update(
			request.Name,
			request.Description,
			request.Color,
			request.ParentCategoryId,
			request.IsActive
		);

		// Save to database
		await _repository.UpdateAsync(category, ct);

		return new CategoryDto()
		{
			Id = category.Id,
			Name = category.Name,
			Description = category.Description,
			Color = category.Color,
			ParentCategoryId = category.ParentCategoryId,
			IsActive = category.IsActive
		};
	}
}

public class UpdateCategoryCommandValidator<T> : AbstractValidator<T> where T : UpdateCategoryCommand
{
	public UpdateCategoryCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID cannot be empty.");

		RuleFor(x => x.Name).NotEmptyMaxLength(64);

		RuleFor(x => x.Description).NotEmptyMaxLength(256);
		//.When(x => !string.IsNullOrWhiteSpace(x.Description));

		RuleFor(x => x.Color).IsHexColor();
	}
}
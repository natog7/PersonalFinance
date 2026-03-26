using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Entities.Interfaces;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record CreateCategoryCommand
(
	string Name = "",
	string? Description = null,
	string Color = "#000000",
	Guid? ParentCategoryId = null
) : IRequest<IdDto<Guid>>, ICategoryFields;

public class CreateCategoryCommandHandler : CommandHandler<CreateCategoryCommand, IdDto<Guid>, ICategoryRepository>
{
	public CreateCategoryCommandHandler(ICategoryRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<IdDto<Guid>> Handle(CreateCategoryCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		Category category = Category.Create(
			_userService.UserId,
			request.Name,
			request.Description,
			request.Color,
			request.ParentCategoryId
		);

		await _repository.AddAsync(category, ct);

		return new IdDto<Guid>
		{
			Id = category.Id
		};
	}
}

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
	public CreateCategoryCommandValidator()
	{
		Include(new CategoryFieldsValidator<CreateCategoryCommand>());
	}
}
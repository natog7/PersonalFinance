using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record CreateCategoryCommand : IRequest<IdDto<Guid>>
{
	public string Name { get; private set; } = string.Empty;
	public string? Description { get; private set; }
	public string Color { get; private set; } = "#000000";
	public Guid? ParentCategoryId { get; private set; }
}

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

public class CreateCategoryCommandValidator<T> : AbstractValidator<T> where T : CreateCategoryCommand
{
	public CreateCategoryCommandValidator()
	{
		RuleFor(x => x.Name).NotEmptyMaxLength(64);

		RuleFor(x => x.Description).NotEmptyMaxLength(256);
		//.When(x => !string.IsNullOrWhiteSpace(x.Description));

		RuleFor(x => x.Color).IsHexColor();
	}
}
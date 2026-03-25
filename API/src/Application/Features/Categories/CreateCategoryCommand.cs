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

	public override async Task<IdDto<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
	{
		if(!_userService.isAuthenticated)
		{
			throw new UnauthorizedAccessException("User must be authenticated by logging in.");
		}

		Category category = Category.Create(
			_userService.UserId,
			request.Name,
			request.Description,
			request.Color,
			request.ParentCategoryId
		);

		await _repository.AddAsync(category, cancellationToken);

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
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class GetCategoryQueryHandler : CommandHandler<GetByIdQuery<CategoryDto?>, CategoryDto?, ICategoryRepository>
{
	public GetCategoryQueryHandler(ICategoryRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<CategoryDto?> Handle(GetByIdQuery<CategoryDto?> request, CancellationToken ct)
	{
		var category = await _repository.GetByIdAsync(request.Id, ct);

		if (category is null)
			return null;

		return new CategoryDto
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

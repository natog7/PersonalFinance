using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record GetCategoriesQuery(string? Name, string? Description, Guid? ParentCategoryId, bool? IsActive) : IRequest<ListResult<CategoryDto>>;

public class GetCategoriesQueryHandler : CommandHandler<GetCategoriesQuery, ListResult<CategoryDto>, ICategoryRepository>
{
	public GetCategoriesQueryHandler(ICategoryRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<ListResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
	{
		return new ListResult<CategoryDto>
		{
			Items = (await _repository.GetFilterAsync(request, ct))
			.Select(x => new CategoryDto
			{
				Id = x.Id,
				Name = x.Name,
				Description = x.Description,
				Color = x.Color,
				ParentCategoryId = x.ParentCategoryId,
				IsActive = x.IsActive
			}).ToList()
		};
	}
}
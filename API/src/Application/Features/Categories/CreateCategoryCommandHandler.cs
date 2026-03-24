using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IdDto<Guid>>
{
	private readonly ICategoryRepository _repository;

	public CreateCategoryCommandHandler(ICategoryRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

	public async Task<IdDto<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
	{
		Category category = Categories.Create(
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

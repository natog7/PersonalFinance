using PersonalFinanceAPI.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Categories;

public class UpdateCategoryHandler(ICategoryRepository repository)
	: IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
	public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct)
	{
		var category = await repository.GetByIdAsync(request.Id, ct)
			?? throw new Exception("Not found.");

		category.Update(
			request.Name,
			request.Description,
			request.Color,
			request.ParentCategoryId,
			request.IsActive
		);

		await repository.UpdateAsync(category, ct);

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

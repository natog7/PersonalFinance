using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description, string Color, Guid? ParentCategoryId, bool IsActive) : IRequest<CategoryDto>;

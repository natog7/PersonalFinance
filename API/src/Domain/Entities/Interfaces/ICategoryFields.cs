using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Domain.Entities.Interfaces;

public interface ICategoryFields
{
	public string? Name { get; }
	public string? Description { get; }
	public string? Color { get; }
	public Guid? ParentCategoryId { get; }
}

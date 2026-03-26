using PersonalFinanceAPI.Domain.Entities.Interfaces;

namespace PersonalFinanceAPI.Application.Features.Shared;

public class EntityFieldsValidator<T> : AbstractValidator<T> where T : IEntityFields<Guid>
{
	public EntityFieldsValidator()
	{
		RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} cannot be empty.");
	}
}

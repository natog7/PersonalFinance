using PersonalFinanceAPI.Domain.Entities.Interfaces;

namespace PersonalFinanceAPI.Application.Features.Shared;

public record DeleteCommand(Guid Id) : IRequest<Unit>, IEntityFields<Guid>;

public class DeleteCommandValidator : AbstractValidator<DeleteCommand>
{
	public DeleteCommandValidator()
	{
		Include(new EntityFieldsValidator<DeleteCommand>());
	}
}
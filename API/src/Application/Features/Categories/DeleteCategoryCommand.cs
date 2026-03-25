using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Categories;

public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;

public class DeleteCategoryCommandHandler : CategoryCommandHandler<DeleteCategoryCommand, Unit>
{
	public DeleteCategoryCommandHandler(ICategoryRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken ct)
	{
		if (!_userService.isAuthenticated)
		{
			throw new UnauthorizedAccessException("User must be authenticated by logging in.");
		}

		var category = await _repository.GetByIdAsync(request.Id, ct)
			?? throw new Exception("Not found.");

		var hasTransactions = await _repository.HasTransactionsAsync(category.Id, ct);
		if (hasTransactions)
		{
			throw new Exception("This category can't be deleted, it has transactions.");
		}

		await _repository.DeleteAsync(category.Id, ct);

		return Unit.Value;
	}
}


public class DeleteCategoryCommandValidator<T> : AbstractValidator<T> where T : DeleteCategoryCommand
{
	public DeleteCategoryCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID cannot be empty.");
	}
}
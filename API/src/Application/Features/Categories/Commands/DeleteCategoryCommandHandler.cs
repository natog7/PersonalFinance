using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Categories.Commands;

public class DeleteCategoryCommandHandler : CommandHandler<DeleteCommand, Unit, ICategoryRepository>
{
	public DeleteCategoryCommandHandler(ICategoryRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<Unit> Handle(DeleteCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

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
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public class DeleteTransactionCommandHandler : CommandHandler<DeleteCommand, Unit, ITransactionRepository>
{
	public DeleteTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<Unit> Handle(DeleteCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		var transaction = await _repository.GetByIdAsync(request.Id, ct)
			?? throw new Exception("Not found.");

		await _repository.DeleteAsync(transaction.Id, ct);

		return Unit.Value;
	}
}
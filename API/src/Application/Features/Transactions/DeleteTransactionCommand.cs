using PersonalFinanceAPI.Application.Features.Categories;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public record DeleteTransactionCommand(Guid Id) : IRequest<Unit>;

public class DeleteTransactionCommandHandler : CommandHandler<DeleteTransactionCommand, Unit, ITransactionRepository>
{
	public DeleteTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken ct)
	{
		CheckAuthenticated();
		var transaction = await _repository.GetByIdAsync(request.Id, ct)
			?? throw new Exception("Not found.");

		await _repository.DeleteAsync(transaction.Id, ct);

		return Unit.Value;
	}
}


public class DeleteTransactionCommandValidator<T> : AbstractValidator<T> where T : DeleteTransactionCommand
{
	public DeleteTransactionCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID cannot be empty.");
	}
}
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Entities.Interfaces;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions.Commands;

public record CreateTransactionCommand
(
	string Title,
	decimal Amount,
	DateOnly Date,
	int Type,
	Guid CategoryId,
	string Currency = "BRL"
) : IRequest<IdDto<Guid>>, ITransactionFields;

public class CreateTransactionCommandHandler : CommandHandler<CreateTransactionCommand, IdDto<Guid>, ITransactionRepository>
{
	public CreateTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<IdDto<Guid>> Handle(CreateTransactionCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		var transaction = Transaction.Create(
			_userService.UserId,
			request.Title,
			Money.Create(request.Amount, request.Currency),
			request.Date,
			(TransactionType)request.Type,
			request.CategoryId
		);

		// Save to database
		await _repository.AddAsync(transaction, ct);

		return new IdDto<Guid>
		{
			Id = transaction.Id
		};
	}
}

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
	public CreateTransactionCommandValidator()
	{
		Include(new TransactionFieldsValidator<CreateTransactionCommand>());
	}
}
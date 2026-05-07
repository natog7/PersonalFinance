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
	TransactionType Type,
	Guid CategoryId,
	RecurrentTransactionData? Recurrent = null
) : IRequest<IdDto<Guid>>, ITransactionFields;

public class CreateTransactionCommandHandler : CommandHandler<CreateTransactionCommand, IdDto<Guid>, ITransactionRepository>
{
	public CreateTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }


	public override async Task<IdDto<Guid>> Handle(CreateTransactionCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		Transaction transaction;

		if (request.Recurrent is not null)
		{
			transaction = RecurrentTransaction.Create(
				_userService.UserId,
				request.Title,
				Money.Create(request.Amount, _userService.Currency),
				request.Date,
				request.Recurrent.EndDate ?? DateOnly.MaxValue,
				request.Type,
				request.CategoryId,
				request.Recurrent.Period
			);
		}
		else
		{
			transaction = Transaction.Create(
				_userService.UserId,
				request.Title,
				Money.Create(request.Amount, _userService.Currency),
				request.Date,
				request.Type,
				request.CategoryId
			);
		}

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

       RuleFor(x => x.Recurrent)
           .SetValidator(new RecurrentTransactionDataValidator())
           .When(x => x.Recurrent is not null);
	}
}
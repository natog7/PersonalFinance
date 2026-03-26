using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.RecurrentTransactions;

public record CreateRecurrentTransactionCommand(
	int Period,
	DateOnly EndDate,
	string Title,
	decimal Amount,
	DateOnly Date,
	int Type,
	Guid CategoryId,
	string Currency = "BRL"
): CreateTransactionCommand(Title, Amount, Date, Type, CategoryId, Currency);

public class CreateRecurrentTransactionCommandHandler : CommandHandler<CreateRecurrentTransactionCommand, IdDto<Guid>, ITransactionRepository>
{
	public CreateRecurrentTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<IdDto<Guid>> Handle(CreateRecurrentTransactionCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		var transaction = RecurrentTransaction.Create(
			_userService.UserId,
			request.Title,
			Money.Create(request.Amount, request.Currency),
			request.Date,
			request.EndDate,
			(TransactionType)request.Type,
			request.CategoryId,
			(RecurrentPeriod)request.Period
		);

		// Save to database
		await _repository.AddAsync(transaction, ct);

		return new IdDto<Guid>
		{
			Id = transaction.Id
		};
	}
}

public class CreateRecurrentTransactionCommandValidator : AbstractValidator<CreateRecurrentTransactionCommand>
{
	public CreateRecurrentTransactionCommandValidator() : base()
	{
		Include(new TransactionFieldsValidator<CreateRecurrentTransactionCommand>());

		RuleFor(x => x.Period).NotEmptyOrNull();

		RuleFor(x => x.EndDate)
			.GreaterThan(x => x.Date)
			.WithMessage("End date must be greater than the transaction date.");
	}
}
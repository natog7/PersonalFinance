using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;
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

public class CreateRecurrentTransactionCommandHandler : IRequestHandler<CreateRecurrentTransactionCommand, IdDto<Guid>>
{
	private readonly ITransactionRepository _repository;

	public CreateRecurrentTransactionCommandHandler(ITransactionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

	public async Task<IdDto<Guid>> Handle(
		CreateRecurrentTransactionCommand request,
		CancellationToken ct)
	{
		var transaction = RecurrentTransaction.Create(
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

		RuleFor(x => x.Period)
			.NotEmpty().WithMessage("Period is required.");

		RuleFor(x => x.EndDate)
			.GreaterThan(x => x.Date)
			.WithMessage("End date must be greater than the transaction date.");
	}
}
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities.Interfaces;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions.Commands;

using PersonalFinanceAPI.Domain.Entities;

public record UpdateTransactionCommand
(
	Guid Id,
	string Title,
	decimal Amount,
	DateOnly Date,
	TransactionType Type,
	Guid CategoryId,
	RecurrentTransactionData? Recurrent = null
) : IRequest<TransactionDto>, IEntityFields<Guid>, ITransactionFields;

public class UpdateTransactionCommandHandler : CommandHandler<UpdateTransactionCommand, TransactionDto, ITransactionRepository>
{
	public UpdateTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<TransactionDto> Handle(UpdateTransactionCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		var transaction = await _repository.GetByIdAsync(request.Id, ct)
			?? throw new Exception("Not found.");


       // If recurrent data is present and transaction is recurrent, update recurrent fields
       if (request.Recurrent is not null && transaction is RecurrentTransaction recurrentTransaction)
       {
           recurrentTransaction.Update(
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
           transaction.Update(
               request.Title,
               Money.Create(request.Amount, _userService.Currency),
               request.Date,
               request.Type,
               request.CategoryId
           );
       }

		// Save to database
		await _repository.UpdateAsync(transaction, ct);

		return new TransactionDto()
		{
			Id = transaction.Id,
			Title = transaction.Title,
			Amount = transaction.Amount.Amount,
			Currency = transaction.Amount.Currency,
			Date = transaction.Date,
			Type = transaction.Type,
			CategoryId = transaction.CategoryId,
			CategoryName = transaction.Category.Name,
			IsRecurrent = transaction.IsRecurrent,
			Recurrent = transaction is RecurrentTransaction rt ? new RecurrentTransactionData(rt.EndDate, rt.Period) : null
		};
	}
}

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
	public UpdateTransactionCommandValidator()
	{
       Include(new EntityFieldsValidator<UpdateTransactionCommand>());
       Include(new TransactionFieldsValidator<UpdateTransactionCommand>());

       RuleFor(x => x.Recurrent)
           .SetValidator(new RecurrentTransactionDataValidator())
           .When(x => x.Recurrent is not null);
	}
}
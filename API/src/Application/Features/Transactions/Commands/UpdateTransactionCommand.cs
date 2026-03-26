using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities.Interfaces;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions.Commands;

public record UpdateTransactionCommand
(
	Guid Id,
	string Title,
	decimal Amount,
	string Currency,
	DateOnly Date,
	int Type,
	Guid CategoryId
) : IRequest<TransactionDto>, IEntityFields<Guid>, ITransactionFields;

public class UpdateTransactionCommandHandler : CommandHandler<UpdateTransactionCommand, TransactionDto, ITransactionRepository>
{
	public UpdateTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<TransactionDto> Handle(UpdateTransactionCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		var transaction = await _repository.GetByIdAsync(request.Id, ct)
			?? throw new Exception("Not found.");

		transaction.Update(
			request.Title,
			Money.Create(request.Amount, request.Currency),
			request.Date,
			(TransactionType)request.Type,
			request.CategoryId
		);

		// Save to database
		await _repository.UpdateAsync(transaction, ct);

		return new TransactionDto()
		{
			Id = transaction.Id,
			Title = transaction.Title,
			Amount = transaction.Amount.Amount,
			Currency = transaction.Amount.Currency,
			Date = transaction.Date,
			Type = (int) transaction.Type,
			CategoryId = transaction.CategoryId,
			CategoryName = transaction.Category.Name,
			IsRecurrent = transaction.IsRecurrent
		};
	}
}

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
	public UpdateTransactionCommandValidator()
	{
		Include(new EntityFieldsValidator<UpdateTransactionCommand>());
		Include(new TransactionFieldsValidator<UpdateTransactionCommand>());
	}
}
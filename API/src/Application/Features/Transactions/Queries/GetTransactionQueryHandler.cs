using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Transactions.Queries;

public class GetTransactionQueryHandler : CommandHandler<GetByIdQuery<TransactionDto?>, TransactionDto?, ITransactionRepository>
{
	public GetTransactionQueryHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<TransactionDto?> Handle(GetByIdQuery<TransactionDto?> request, CancellationToken ct)
	{
		var transaction = await _repository.GetByIdAsync(request.Id, ct);

		if (transaction is null)
			return null;

		return new TransactionDto
		{
			Id = transaction.Id,
			Title = transaction.Title,
			Amount = transaction.Amount.Amount,
			Currency = transaction.Amount.Currency,
			Date = transaction.Date,
			Type = (int)transaction.Type,
			CategoryId = transaction.CategoryId,
			CategoryName = transaction.Category.Name,
			IsRecurrent = transaction.IsRecurrent
		};
	}
}
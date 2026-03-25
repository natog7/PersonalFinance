using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public class GetTransactionsQuery : IRequest<GetTransactionsResult>
{
    public string? Title { get; set; }
	public DateOnlyPeriod? Date { get; set; }
	public TransactionType? Type { get; set; }
	public List<Guid>? CategoryIds { get; set; }
}

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, GetTransactionsResult>
{
	private readonly ITransactionRepository _repository;

	public GetTransactionsQueryHandler(ITransactionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

	public async Task<GetTransactionsResult> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
	{
		return new GetTransactionsResult
		{
			Transactions = (await _repository.GetFilterAsync(request, cancellationToken))
			.Select(t => new TransactionDto
			{
				Id = t.Id,
				Title = t.Title,
				Amount = t.Amount.Amount,
				Currency = t.Amount.Currency,
				Date = t.Date,
				Type = (int)t.Type,
				CategoryId = t.CategoryId,
				CategoryName = t.Category.Name,
				IsRecurrent = t.IsRecurrent
			}).ToList()
		};
	}
}
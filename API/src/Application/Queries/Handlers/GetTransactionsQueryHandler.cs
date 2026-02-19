using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Repositories;

namespace PersonalFinanceAPI.Application.Queries.Handlers;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, GetTransactionsResult>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionsQueryHandler(ITransactionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetTransactionsResult> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        return new GetTransactionsResult { 
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

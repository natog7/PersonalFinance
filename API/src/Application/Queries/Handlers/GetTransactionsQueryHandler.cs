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
        //var query = _repository.Transactions
        //    .Include(t => t.Category)
        //    .AsQueryable();

        var query = _repository.GetQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(t => t.Title.Contains(request.Name));
        }

        if (request.CategoryIds?.Any() == true)
        {
            query = query.Where(t => request.CategoryIds.Contains(t.CategoryId));
        }

        var transactions = await query
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
            })
            .ToListAsync(cancellationToken);

        return new GetTransactionsResult { Transactions = transactions };
    }
}

using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Repositories;

namespace PersonalFinanceAPI.Application.Queries.Handlers;

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionDto?>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionQueryHandler(ITransactionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<TransactionDto?> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        //var transaction = await _repository.Transactions
        //    .Include(t => t.Category)
        //    .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        var transaction = await _repository.GetByIdAsync(request.Id, cancellationToken);

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

using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, IdDto<Guid>>
{
    private readonly ITransactionRepository _repository;

    public CreateTransactionCommandHandler(ITransactionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IdDto<Guid>> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var transaction = Transaction.Create(
            request.Title,
			Money.Create(request.Amount, request.Currency),
            request.Date,
			(TransactionType)request.Type,
            request.CategoryId
        );

        // Save to database
        await _repository.AddAsync(transaction, cancellationToken);

        return new IdDto<Guid>
        {
            Id = transaction.Id
        };
    }
}

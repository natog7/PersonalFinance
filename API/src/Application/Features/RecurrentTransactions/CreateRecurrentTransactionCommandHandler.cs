using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.RecurrentTransactions;

public class CreateRecurrentTransactionCommandHandler : IRequestHandler<CreateRecurrentTransactionCommand, IdDto<Guid>>
{
    private readonly ITransactionRepository _repository;

    public CreateRecurrentTransactionCommandHandler(ITransactionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IdDto<Guid>> Handle(
		CreateRecurrentTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var transaction = RecurrentTransaction.Create(
            request.Title,
			Money.Create(request.Amount, request.Currency),
            request.Date,
            request.EndDate,
			(TransactionType) request.Type,
            request.CategoryId,
            (RecurrentPeriod) request.Period
        );

        // Save to database
        await _repository.AddAsync(transaction, cancellationToken);

        return new IdDto<Guid>
        {
            Id = transaction.Id
        };
    }
}

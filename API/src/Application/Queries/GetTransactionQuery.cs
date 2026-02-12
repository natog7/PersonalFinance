using PersonalFinanceAPI.Application.Features.Transactions;

namespace PersonalFinanceAPI.Application.Queries;

public class GetTransactionQuery : IRequest<TransactionDto?>
{
    public Guid Id { get; set; }
}

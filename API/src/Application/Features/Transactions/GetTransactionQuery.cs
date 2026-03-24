namespace PersonalFinanceAPI.Application.Features.Transactions;

public class GetTransactionQuery : IRequest<TransactionDto?>
{
    public Guid Id { get; set; }
}

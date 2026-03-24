namespace PersonalFinanceAPI.Application.Features.Transactions;

public class GetTransactionsResult
{
	public List<TransactionDto> Transactions { get; set; } = new();
}
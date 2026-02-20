using PersonalFinanceAPI.Application.Features.Transactions;

namespace PersonalFinanceAPI.Application.Queries;

public class GetTransactionsResult
{
	public List<TransactionDto> Transactions { get; set; } = new();
}
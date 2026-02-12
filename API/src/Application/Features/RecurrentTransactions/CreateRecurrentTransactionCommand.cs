using PersonalFinanceAPI.Application.Features.Transactions;

namespace PersonalFinanceAPI.Application.Features.RecurrentTransactions;

public record CreateRecurrentTransactionCommand : CreateTransactionCommand
{
	public int Period { get; set; }
	public DateOnly EndDate { get; set; }
}

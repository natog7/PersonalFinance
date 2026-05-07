using PersonalFinanceAPI.Domain.Entities.Interfaces;
using PersonalFinanceAPI.Domain.Enums;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public record RecurrentTransactionData(DateOnly? EndDate, RecurrentPeriod Period);

public record TransactionDto : IdDto<Guid>, ITransactionFields
{
	public string Title { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public string Currency { get; set; } = string.Empty;
	public DateOnly Date { get; set; }
	public TransactionType Type { get; set; }
	public Guid CategoryId { get; set; }
	public string CategoryName { get; set; } = string.Empty;
	public bool IsRecurrent { get; set; }
	public RecurrentTransactionData? Recurrent { get; set; } = null;
}

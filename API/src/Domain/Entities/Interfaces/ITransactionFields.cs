using PersonalFinanceAPI.Domain.Enums;

namespace PersonalFinanceAPI.Domain.Entities.Interfaces;

public interface ITransactionFields
{
	public string Title { get; }
	public decimal Amount { get; }
	public DateOnly Date { get; }
	public TransactionType Type { get; }
	public Guid CategoryId { get; }
}

namespace PersonalFinanceAPI.Domain.Entities.Interfaces;

public interface ITransactionFields
{
	public string Title { get; }
	public decimal Amount { get; }
	public string Currency { get; }
	public DateOnly Date { get; }
	public int Type { get; }
	public Guid CategoryId { get; }
}

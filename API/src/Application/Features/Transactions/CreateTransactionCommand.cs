namespace PersonalFinanceAPI.Application.Features.Transactions;

/// <summary>
/// Command to create a new transaction.
/// </summary>
public record CreateTransactionCommand : IRequest<IdDto<Guid>>
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public DateOnly Date { get; set; }
    public int Type { get; set; }
    public Guid CategoryId { get; set; }
}

using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public class GetTransactionsQuery : IRequest<GetTransactionsResult>
{
    public string? Title { get; set; }
	public DateOnlyPeriod? Date { get; set; }
	public TransactionType? Type { get; set; }
	public List<Guid>? CategoryIds { get; set; }
}

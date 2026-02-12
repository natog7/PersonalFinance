using PersonalFinanceAPI.Application.Features.Transactions;

namespace PersonalFinanceAPI.Application.Queries;

public class GetTransactionsQuery : IRequest<GetTransactionsResult>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<Guid>? CategoryIds { get; set; }
}

public class GetTransactionsResult
{
    public List<TransactionDto> Transactions { get; set; } = new();
}

using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions.Queries;

public record GetTransactionsQuery
(
	string? Title = null,
	DateOnlyPeriod? Date = null,
	TransactionType? Type = null,
	List<Guid>? CategoryIds = null
) : IRequest<ListResult<TransactionDto>>;

public class GetTransactionsQueryHandler : CommandHandler<GetTransactionsQuery, ListResult<TransactionDto>, ITransactionRepository>
{
	public GetTransactionsQueryHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<ListResult<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken ct)
	{
		return new ListResult<TransactionDto>
		{
			Items = (await _repository.GetFilterAsync(request, ct))
			.Select(x => new TransactionDto
			{
				Id = x.Id,
				Title = x.Title,
				Amount = x.Amount.Amount,
				Currency = x.Amount.Currency,
				Date = x.Date,
				Type = (int)x.Type,
				CategoryId = x.CategoryId,
				CategoryName = x.Category.Name,
				IsRecurrent = x.IsRecurrent
			}).ToList()
		};
	}
}
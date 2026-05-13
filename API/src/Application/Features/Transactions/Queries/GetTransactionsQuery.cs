using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Features.Auth.Commands;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions.Queries;

public record GetTransactionsQuery
(
	string? Title = null,
	DateOnlyPeriod? Date = null,
	TransactionType? Type = null,
	string? Currency = null,
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
				Type = x.Type,
				CategoryId = x.CategoryId,
				CategoryName = x.Category.Name,
				IsRecurrent = x.IsRecurrent,
				Recurrent = x is RecurrentTransaction rt ? new RecurrentTransactionData(rt.EndDate, rt.Period) : null
			}).ToList()
		};
	}
}

public class GetTransactionsQueryValidator : AbstractValidator<GetTransactionsQuery>
{
	public GetTransactionsQueryValidator()
	{
		RuleFor(x => x.Title)
			.MaxLengthNull(256)
			.When(x => !string.IsNullOrWhiteSpace(x.Title));

		RuleFor(x => x.Currency)
			.MaxLengthNull(3)
			.When(x => !string.IsNullOrWhiteSpace(x.Currency));

		RuleFor(x => x.CategoryIds)
			.Must(list => list == null || list.All(guid => guid != Guid.Empty))
			.WithMessage("{PropertyName} must not contain empty GUIDs.");

		RuleFor(x => x.Date)
			.IsDateRangeNull()
			.When(x => x.Date != null);
	}
}
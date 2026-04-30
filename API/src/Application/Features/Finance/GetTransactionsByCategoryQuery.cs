using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Finance;

public record GetTransactionsByCategoryQuery(
    DateOnlyPeriod Date,
    TransactionType? Type,
    List<Guid>? CategoryIds
) : IRequest<ListResult<CategoryTransactionSumDto>>;

public record CategoryTransactionSumDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
}



public class GetTransactionsByCategoryQueryHandler : CommandHandler<GetTransactionsByCategoryQuery, ListResult<CategoryTransactionSumDto>, ITransactionRepository>
{
    public GetTransactionsByCategoryQueryHandler(
        ITransactionRepository repository,
        ICurrentUserService userService)
        : base(repository, userService)
    {
    }

    public override async Task<ListResult<CategoryTransactionSumDto>> Handle(GetTransactionsByCategoryQuery request, CancellationToken ct)
    {
		ListResult<CategoryTransactionSumDto> transactions = new();
		transactions.Items = await _repository.GetTransactionsByCategoryAsync(
            request.Date,
            request.Type,
            request.CategoryIds,
            ct);

        return transactions;
    }
}

public class GetTransactionsByCategoryQueryValidator : AbstractValidator<GetTransactionsByCategoryQuery>
{
    public GetTransactionsByCategoryQueryValidator()
    {
        RuleFor(x => x.Date).NotNull();
    }
}

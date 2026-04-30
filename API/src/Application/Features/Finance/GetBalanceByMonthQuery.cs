using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Finance;

public record GetBalanceByMonthQuery(
    DateOnlyPeriod Date,
    List<Guid>? CategoryIds
) : IRequest<ListResult<BalanceByMonthDto>>;

public record BalanceByMonthDto
{
    public DateOnly Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal TotalIncomeGrowthPercentage { get; set; }
    public decimal TotalExpenseGrowthPercentage { get; set; }
    public decimal TotalGrowthPercentage { get; set; }
}

public class GetBalanceByMonthQueryHandler : CommandHandler<GetBalanceByMonthQuery, ListResult<BalanceByMonthDto>, ITransactionRepository>
{
    public GetBalanceByMonthQueryHandler(
        ITransactionRepository repository,
        ICurrentUserService userService)
        : base(repository, userService)
    {
    }

    public override async Task<ListResult<BalanceByMonthDto>> Handle(GetBalanceByMonthQuery request, CancellationToken ct)
    {
        ListResult<BalanceByMonthDto> result = new();
        result.Items = await _repository.GetBalanceByMonthAsync(
            request.Date,
            request.CategoryIds,
            ct);

        return result;
    }
}

public class GetBalanceByMonthQueryValidator : AbstractValidator<GetBalanceByMonthQuery>
{
    public GetBalanceByMonthQueryValidator()
    {
        RuleFor(x => x.Date).NotNull();
    }
}

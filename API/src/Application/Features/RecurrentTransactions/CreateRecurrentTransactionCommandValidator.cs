using PersonalFinanceAPI.Application.Features.Transactions;

namespace PersonalFinanceAPI.Application.Features.RecurrentTransactions;

public class CreateRecurrentTransactionCommandValidator : CreateTransactionCommandValidator<CreateRecurrentTransactionCommand>
{
    public CreateRecurrentTransactionCommandValidator() : base()
    {
        RuleFor(x => x.Period)
            .NotEmpty().WithMessage("Period is required.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.Date)
            .WithMessage("End date must be greater than the transaction date.");
    }
}
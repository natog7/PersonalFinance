namespace PersonalFinanceAPI.Application.Features.Transactions;

public class CreateTransactionCommandValidator<T> : AbstractValidator<T> where T : CreateTransactionCommand
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(256).WithMessage("Title cannot exceed 256 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency code must be 3 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.Type)
            .Must(t => t == 1 || t == 2).WithMessage("Type must be 1 (Income) or 2 (Expense).");
    }
}
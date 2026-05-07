using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Features.Transactions.Commands;
using PersonalFinanceAPI.Domain.Entities.Interfaces;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public class TransactionFieldsValidator<T> : AbstractValidator<T> where T : ITransactionFields
{
	public TransactionFieldsValidator()
	{
		RuleFor(x => x.Title).NotEmptyMaxLength(64);

		RuleFor(x => x.Amount).GreaterThan(0).WithMessage("{PropertyName} must be greater than zero.");

		RuleFor(x => x.Type).IsInEnum().WithMessage("{PropertyName} must be 0 (Income) or 1 (Expense).");

		RuleFor(x => x.CategoryId).NotEmpty().WithMessage("{PropertyName} is required.");
	}
}

public class RecurrentTransactionDataValidator : AbstractValidator<RecurrentTransactionData>
{
	public RecurrentTransactionDataValidator()
	{
		RuleFor(x => x.Period).IsInEnum().WithMessage("{PropertyName} must be a valid RecurrentPeriod.");

		RuleFor(x => x.EndDate)
			.Must((data, endDate) => endDate == null || endDate > DateOnly.FromDateTime(DateTime.Today))
			.WithMessage("{PropertyName} must be in the future or null.");
	}
}

using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Domain.Entities.Interfaces;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public class TransactionFieldsValidator<T> : AbstractValidator<T> where T : ITransactionFields
{
	public TransactionFieldsValidator()
	{
		RuleFor(x => x.Title).NotEmptyMaxLength(64);

		RuleFor(x => x.Amount).GreaterThan(0).WithMessage("{PropertyName} must be greater than zero.");

		RuleFor(x => x.Currency).NotEmptyLength(3);

		RuleFor(x => x.Type).Must(t => t == 1 || t == 2).WithMessage("{PropertyName} must be 1 (Income) or 2 (Expense).");

		RuleFor(x => x.CategoryId).NotEmpty().WithMessage("{PropertyName} is required.");
	}
}

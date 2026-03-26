using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public record CreateTransactionCommand : IRequest<IdDto<Guid>>
{
	public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public DateOnly Date { get; set; }
    public int Type { get; set; }
    public Guid CategoryId { get; set; }
}

public class CreateTransactionCommandHandler : CommandHandler<CreateTransactionCommand, IdDto<Guid>, ITransactionRepository>
{
	public CreateTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<IdDto<Guid>> Handle(CreateTransactionCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		var transaction = Transaction.Create(
			_userService.UserId,
			request.Title,
			Money.Create(request.Amount, request.Currency),
			request.Date,
			(TransactionType)request.Type,
			request.CategoryId
		);

		// Save to database
		await _repository.AddAsync(transaction, ct);

		return new IdDto<Guid>
		{
			Id = transaction.Id
		};
	}
}

public class CreateTransactionCommandValidator<T> : AbstractValidator<T> where T : CreateTransactionCommand
{
	public CreateTransactionCommandValidator()
	{
		RuleFor(x => x.Title).NotEmptyMaxLength(64);

		RuleFor(x => x.Amount).GreaterThan(0).WithMessage("{PropertyName} must be greater than zero.");

		RuleFor(x => x.Currency).NotEmptyLength(3);

		RuleFor(x => x.CategoryId).NotEmpty().WithMessage("{PropertyName} is required.");

		RuleFor(x => x.Type).Must(t => t == 1 || t == 2).WithMessage("{PropertyName} must be 1 (Income) or 2 (Expense).");
	}
}
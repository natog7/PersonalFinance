using PersonalFinanceAPI.Application.Features.Categories;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.Services;
using PersonalFinanceAPI.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public record UpdateTransactionCommand(Guid Id, string Title, decimal Amount, string Currency, DateOnly Date, int Type, Guid CategoryId) : IRequest<TransactionDto>;


public class UpdateTransactionCommandHandler : CommandHandler<UpdateTransactionCommand, TransactionDto, ITransactionRepository>
{
	public UpdateTransactionCommandHandler(ITransactionRepository repository, ICurrentUserService userService) : base(repository, userService) { }

	public override async Task<TransactionDto> Handle(UpdateTransactionCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		var transaction = await _repository.GetByIdAsync(request.Id, ct)
			?? throw new Exception("Not found.");

		transaction.Update(
			request.Title,
			Money.Create(request.Amount, request.Currency),
			request.Date,
			(TransactionType)request.Type,
			request.CategoryId
		);

		// Save to database
		await _repository.UpdateAsync(transaction, ct);

		return new TransactionDto()
		{
			Id = transaction.Id,
			Title = transaction.Title,
			Amount = transaction.Amount.Amount,
			Currency = transaction.Amount.Currency,
			Date = transaction.Date,
			Type = (int) transaction.Type,
			CategoryId = transaction.CategoryId,
			CategoryName = transaction.Category.Name,
			IsRecurrent = transaction.IsRecurrent
		};
	}
}

public class UpdateTransactionCommandValidator<T> : AbstractValidator<T> where T : UpdateTransactionCommand
{
	public UpdateTransactionCommandValidator()
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
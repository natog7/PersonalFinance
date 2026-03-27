using PersonalFinanceAPI.Domain.Enums;

namespace PersonalFinanceAPI.Domain.Events;

public record TransactionChangedEvent
(
	Guid UserId,
	string Title,
	decimal Amount,
	string Currency,
	DateOnly Date,
	TransactionType Type,
	bool IsRecurrent,
	Guid CategoryId,
	bool IsDeletion = false
);

using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Domain.Entities;

/// <summary>
/// Represents a budget for a category.
/// </summary>
public class RecurrentTransaction : Transaction
{
	public RecurrentPeriod Period { get; private set; }
    public DateOnly? EndDate { get; private set; }

    private RecurrentTransaction() { }

	/// <summary>
	/// Creates a new transaction.
	/// </summary>
	public static RecurrentTransaction Create(string title, Money amount, DateOnly date, DateOnly endDate, TransactionType type,
		Guid categoryId, RecurrentPeriod period)
	{
		CheckCreate(title, amount, date);

		return new RecurrentTransaction
		{
			Id = Guid.NewGuid(),
			Title = title.Trim(),
			Amount = amount,
			Date = date,
			EndDate = endDate,
			Type = type,
			CategoryId = categoryId,
			Period = period,
			CreatedAt = DateTime.UtcNow
		};
	}
}

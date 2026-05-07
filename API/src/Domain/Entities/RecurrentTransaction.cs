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
	public static RecurrentTransaction Create(Guid? userId, string title, Money amount, DateOnly date, DateOnly endDate, TransactionType type,
		Guid categoryId, RecurrentPeriod period)
	{
		CheckCreate(title, amount, date);

		return new RecurrentTransaction
		{
			Id = Guid.NewGuid(),
			UserId = userId,
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

	public void Update(string? title, Money amount, DateOnly? date, DateOnly? endDate, TransactionType? type,
		Guid? categoryId, RecurrentPeriod? period)
	{
		if (!string.IsNullOrWhiteSpace(title))
			Title = title.Trim();
		if (amount != null)
			Amount.Update(amount);
		if (date.HasValue)
			Date = date.Value;
		if (endDate.HasValue)
			EndDate = endDate.Value;
		if (type.HasValue)
			Type = type.Value;
		if (categoryId.HasValue)
			categoryId = categoryId.Value;
		if (period.HasValue)
			Period = period.Value;

		UpdatedAt = DateTime.UtcNow;
	}
}

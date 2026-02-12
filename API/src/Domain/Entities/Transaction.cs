using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Domain.ValueObjects;

namespace PersonalFinanceAPI.Domain.Entities;

/// <summary>
/// Represents a financial transaction in the system.
/// </summary>
/// 
public class Transaction : Entity<Guid>
{
    public string Title { get; protected set; } = string.Empty;
    public Money Amount { get; protected set; } = null!;
    public DateOnly Date { get; protected set; }
    public TransactionType Type { get; protected set; }
    public bool IsRecurrent => this is RecurrentTransaction;
	public Guid CategoryId { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string? IdempotencyHash { get; protected set; }

    // Navigation properties
    public Category Category { get; protected set; } = null!;

    protected Transaction() { }

    protected static void CheckCreate(string title, Money amount, DateOnly date)
	{
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Title cannot be empty.", nameof(title));

		if (amount is null)
			throw new ArgumentNullException(nameof(amount));

		if (date > DateOnly.FromDateTime(DateTime.UtcNow))
			throw new ArgumentException("Transaction date cannot be in the future.", nameof(date));
	}

    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    public static Transaction Create(string title, Money amount, DateOnly date, TransactionType type,
        Guid categoryId)
    {
		CheckCreate(title, amount, date);

		return new Transaction
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Amount = amount,
            Date = date,
            Type = type,
            CategoryId = categoryId,
            CreatedAt = DateTime.UtcNow
        };
	}

	/// <summary>
	/// Updates the transaction amount and category.
	/// </summary>
	public void Update(Money amount, Guid categoryId)
    {
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Generates a hash for idempotency checking (used in statement imports).
    /// </summary>
    public string GenerateIdempotencyHash()
    {
        var combined = $"{Date:yyyy-MM-dd}{Amount.Amount}{Title}";
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combined));
        return Convert.ToBase64String(hash);
    }
}

namespace UnitTests.Domain;

public class TransactionTests
{
	[Fact]
	public void Create_WithValidData_ReturnsTransactionInstance()
	{
		// Arrange
		var title = "Salary Payment";
		var amount = Money.Create(5000m, "BRL");
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var type = TransactionType.Income;
		var categoryId = Guid.NewGuid();

		// Act
		var transaction = Transaction.Create(title, amount, date, type, categoryId);

		// Assert
		Assert.NotEqual(Guid.Empty, transaction.Id);
		Assert.Equal(title, transaction.Title);
		Assert.Equal(amount, transaction.Amount);
		Assert.Equal(date, transaction.Date);
		Assert.Equal(type, transaction.Type);
		Assert.Equal(categoryId, transaction.CategoryId);
		Assert.Equal(DateTime.UtcNow.Date, transaction.CreatedAt.Date);
	}

	[Fact]
	public void Create_WithEmptyTitle_ThrowsArgumentException()
	{
		// Arrange
		var amount = Money.Create(100m, "BRL");
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => 
			Transaction.Create("", amount, date, TransactionType.Expense, Guid.NewGuid()));
		Assert.Contains("Title cannot be empty", ex.Message);
	}

	[Fact]
	public void Create_WithWhitespaceTitle_ThrowsArgumentException()
	{
		// Arrange
		var amount = Money.Create(100m, "BRL");
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => 
			Transaction.Create("   ", amount, date, TransactionType.Expense, Guid.NewGuid()));
		Assert.Contains("Title cannot be empty", ex.Message);
	}

	[Fact]
	public void Create_WithNullAmount_ThrowsArgumentNullException()
	{
		// Arrange
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => 
			Transaction.Create("Test", null!, date, TransactionType.Expense, Guid.NewGuid()));
		Assert.Equal("amount", ex.ParamName);
	}

	[Fact]
	public void Create_WithFutureDate_ThrowsArgumentException()
	{
		// Arrange
		var amount = Money.Create(100m, "BRL");
		var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => 
			Transaction.Create("Test", amount, futureDate, TransactionType.Expense, Guid.NewGuid()));
		Assert.Contains("Transaction date cannot be in the future", ex.Message);
	}

	[Fact]
	public void Create_TrimsTitleWhitespace()
	{
		// Arrange
		var amount = Money.Create(100m, "BRL");
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

		// Act
		var transaction = Transaction.Create("  Test Title  ", amount, date, TransactionType.Expense, Guid.NewGuid());

		// Assert
		Assert.Equal("Test Title", transaction.Title);
	}

	[Fact]
	public void Update_WithValidData_UpdatesTransaction()
	{
		// Arrange
		var transaction = Transaction.Create("Test", Money.Create(100m, "BRL"), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Expense, Guid.NewGuid());
		var newAmount = Money.Create(150m, "BRL");
		var newCategoryId = Guid.NewGuid();

		// Act
		transaction.Update(newAmount, newCategoryId);

		// Assert
		Assert.Equal(newAmount, transaction.Amount);
		Assert.Equal(newCategoryId, transaction.CategoryId);
		Assert.NotNull(transaction.UpdatedAt);
	}

	[Fact]
	public void Update_WithNullAmount_ThrowsArgumentNullException()
	{
		// Arrange
		var transaction = Transaction.Create("Test", Money.Create(100m, "BRL"), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Expense, Guid.NewGuid());

		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => transaction.Update(null!, Guid.NewGuid()));
		Assert.Equal("amount", ex.ParamName);
	}

	[Fact]
	public void IsRecurrent_WithRegularTransaction_ReturnsFalse()
	{
		// Arrange
		var transaction = Transaction.Create("Test", Money.Create(100m, "BRL"), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Expense, Guid.NewGuid());

		// Act & Assert
		Assert.False(transaction.IsRecurrent);
	}

	[Fact]
	public void GenerateIdempotencyHash_WithSameData_ReturnsSameHash()
	{
		// Arrange
		var transaction1 = Transaction.Create("Test", Money.Create(100m, "BRL"), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Expense, Guid.NewGuid());
		var transaction2 = Transaction.Create("Test", Money.Create(100m, "BRL"), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Expense, Guid.NewGuid());

		// Act
		var hash1 = transaction1.GenerateIdempotencyHash();
		var hash2 = transaction2.GenerateIdempotencyHash();

		// Assert
		Assert.Equal(hash1, hash2);
	}

	[Fact]
	public void GenerateIdempotencyHash_WithDifferentData_ReturnsDifferentHash()
	{
		// Arrange
		var transaction1 = Transaction.Create("Test1", Money.Create(100m, "BRL"), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Expense, Guid.NewGuid());
		var transaction2 = Transaction.Create("Test2", Money.Create(100m, "BRL"), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Expense, Guid.NewGuid());

		// Act
		var hash1 = transaction1.GenerateIdempotencyHash();
		var hash2 = transaction2.GenerateIdempotencyHash();

		// Assert
		Assert.NotEqual(hash1, hash2);
	}
}

namespace UnitTests.Domain;

public class RecurrentTransactionTests
{
	[Fact]
	public void Create_WithValidData_ReturnsRecurrentTransactionInstance()
	{
		// Arrange
		var title = "Monthly Rent";
		var amount = Money.Create(1500m, "BRL");
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12));
		var type = TransactionType.Expense;
		var categoryId = Guid.NewGuid();
		var period = RecurrentPeriod.Monthly;

		// Act
		var transaction = RecurrentTransaction.Create(title, amount, date, endDate, type, categoryId, period);

		// Assert
		Assert.NotEqual(Guid.Empty, transaction.Id);
		Assert.Equal(title, transaction.Title);
		Assert.Equal(amount, transaction.Amount);
		Assert.Equal(date, transaction.Date);
		Assert.Equal(endDate, transaction.EndDate);
		Assert.Equal(type, transaction.Type);
		Assert.Equal(categoryId, transaction.CategoryId);
		Assert.Equal(period, transaction.Period);
		Assert.True(transaction.IsRecurrent);
	}

	[Fact]
	public void Create_WithEmptyTitle_ThrowsArgumentException()
	{
		// Arrange
		var amount = Money.Create(100m, "BRL");
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => 
			RecurrentTransaction.Create("", amount, date, endDate, TransactionType.Expense, Guid.NewGuid(), RecurrentPeriod.Monthly));
		Assert.Contains("Title cannot be empty", ex.Message);
	}

	[Fact]
	public void Create_WithNullAmount_ThrowsArgumentNullException()
	{
		// Arrange
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));

		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => 
			RecurrentTransaction.Create("Test", null!, date, endDate, TransactionType.Expense, Guid.NewGuid(), RecurrentPeriod.Monthly));
		Assert.Equal("amount", ex.ParamName);
	}

	[Fact]
	public void Create_WithFutureDate_ThrowsArgumentException()
	{
		// Arrange
		var amount = Money.Create(100m, "BRL");
		var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
		var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));

		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => 
			RecurrentTransaction.Create("Test", amount, futureDate, endDate, TransactionType.Expense, Guid.NewGuid(), RecurrentPeriod.Monthly));
		Assert.Contains("Transaction date cannot be in the future", ex.Message);
	}

	[Fact]
	public void Create_WithDifferentPeriods_ReturnsValidRecurrentTransaction()
	{
		// Arrange
		var amount = Money.Create(100m, "BRL");
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12));
		var categoryId = Guid.NewGuid();

		// Act
		var weekly = RecurrentTransaction.Create("Test", amount, date, endDate, TransactionType.Income, categoryId, RecurrentPeriod.Weekly);
		var monthly = RecurrentTransaction.Create("Test", amount, date, endDate, TransactionType.Income, categoryId, RecurrentPeriod.Monthly);
		var yearly = RecurrentTransaction.Create("Test", amount, date, endDate, TransactionType.Income, categoryId, RecurrentPeriod.Yearly);

		// Assert
		Assert.Equal(RecurrentPeriod.Weekly, weekly.Period);
		Assert.Equal(RecurrentPeriod.Monthly, monthly.Period);
		Assert.Equal(RecurrentPeriod.Yearly, yearly.Period);
	}

	[Fact]
	public void IsRecurrent_WithRecurrentTransaction_ReturnsTrue()
	{
		// Arrange
		var transaction = RecurrentTransaction.Create("Test", Money.Create(100m, "BRL"), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), 
			DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)), 
			TransactionType.Expense, Guid.NewGuid(), RecurrentPeriod.Monthly);

		// Act & Assert
		Assert.True(transaction.IsRecurrent);
	}
}

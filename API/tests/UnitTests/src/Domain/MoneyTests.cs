namespace UnitTests.Domain;

public class MoneyTests
{
	[Fact]
	public void Create_WithValidAmount_ReturnsMoneyInstance()
	{
		// Act
		var money = Money.Create(100.50m, "BRL");

		// Assert
		Assert.Equal(100.50m, money.Amount);
		Assert.Equal("BRL", money.Currency);
	}

	[Fact]
	public void Create_RoundsAmountToTwoDecimals()
	{
		// Act
		var money = Money.Create(100.555m, "USD");

		// Assert
		Assert.Equal(100.56m, money.Amount);
	}

	[Fact]
	public void Create_WithNegativeAmount_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => Money.Create(-50m, "BRL"));
		Assert.Contains("Amount cannot be negative", ex.Message);
	}

	[Fact]
	public void Create_WithEmptyCurrency_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => Money.Create(100m, ""));
		Assert.Contains("Currency code cannot be empty", ex.Message);
	}

	[Fact]
	public void Create_WithDefaultCurrency_UsesBRL()
	{
		// Act
		var money = Money.Create(100m);

		// Assert
		Assert.Equal("BRL", money.Currency);
	}

	[Fact]
	public void Create_NormalizesCurrencyToUpperCase()
	{
		// Act
		var money = Money.Create(100m, "brl");

		// Assert
		Assert.Equal("BRL", money.Currency);
	}

	[Fact]
	public void Add_WithSameCurrency_ReturnsSum()
	{
		// Arrange
		var money1 = Money.Create(100m, "BRL");
		var money2 = Money.Create(50m, "BRL");

		// Act
		var result = money1.Add(money2);

		// Assert
		Assert.Equal(150m, result.Amount);
		Assert.Equal("BRL", result.Currency);
	}

	[Fact]
	public void Add_WithDifferentCurrency_ThrowsInvalidOperationException()
	{
		// Arrange
		var money1 = Money.Create(100m, "BRL");
		var money2 = Money.Create(50m, "USD");

		// Act & Assert
		var ex = Assert.Throws<InvalidOperationException>(() => money1.Add(money2));
		Assert.Contains("Cannot add amounts in different currencies", ex.Message);
	}

	[Fact]
	public void Subtract_WithSameCurrency_ReturnsDifference()
	{
		// Arrange
		var money1 = Money.Create(100m, "BRL");
		var money2 = Money.Create(30m, "BRL");

		// Act
		var result = money1.Subtract(money2);

		// Assert
		Assert.Equal(70m, result.Amount);
		Assert.Equal("BRL", result.Currency);
	}

	[Fact]
	public void Subtract_WithDifferentCurrency_ThrowsInvalidOperationException()
	{
		// Arrange
		var money1 = Money.Create(100m, "BRL");
		var money2 = Money.Create(50m, "USD");

		// Act & Assert
		var ex = Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
		Assert.Contains("Cannot subtract amounts in different currencies", ex.Message);
	}

	[Fact]
	public void Subtract_WhenResultIsNegative_ThrowsInvalidOperationException()
	{
		// Arrange
		var money1 = Money.Create(30m, "BRL");
		var money2 = Money.Create(100m, "BRL");

		// Act & Assert
		var ex = Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
		Assert.Contains("Result cannot be negative", ex.Message);
	}

	[Fact]
	public void Equals_WithSameAmountAndCurrency_ReturnsTrue()
	{
		// Arrange
		var money1 = Money.Create(100m, "BRL");
		var money2 = Money.Create(100m, "BRL");

		// Act & Assert
		Assert.Equal(money1, money2);
	}

	[Fact]
	public void Equals_WithDifferentAmount_ReturnsFalse()
	{
		// Arrange
		var money1 = Money.Create(100m, "BRL");
		var money2 = Money.Create(50m, "BRL");

		// Act & Assert
		Assert.NotEqual(money1, money2);
	}

	[Fact]
	public void Equals_WithDifferentCurrency_ReturnsFalse()
	{
		// Arrange
		var money1 = Money.Create(100m, "BRL");
		var money2 = Money.Create(100m, "USD");

		// Act & Assert
		Assert.NotEqual(money1, money2);
	}

	[Fact]
	public void GetHashCode_WithEqualObjects_ReturnsSameHashCode()
	{
		// Arrange
		var money1 = Money.Create(100m, "BRL");
		var money2 = Money.Create(100m, "BRL");

		// Act & Assert
		Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
	}
}

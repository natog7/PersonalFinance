namespace UnitTests.Application.Features.RecurrentTransactions;

public class CreateRecurrentTransactionCommandValidatorTests
{
	private readonly CreateRecurrentTransactionCommandValidator _validator = new();

	[Fact]
	public void Validate_WithValidCommand_HasNoErrors()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 1500m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 2, // Expense
			CategoryId = Guid.NewGuid(),
			Period = 2, // Monthly
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.True(result.IsValid);
	}

	[Fact]
	public void Validate_WithEmptyTitle_HasErrors()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "",
			Amount = 1500m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 2,
			CategoryId = Guid.NewGuid(),
			Period = 2,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Title");
	}

	[Fact]
	public void Validate_WithZeroAmount_HasErrors()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 0m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 2,
			CategoryId = Guid.NewGuid(),
			Period = 2,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Amount");
	}

	[Fact]
	public void Validate_WithInvalidCurrency_HasErrors()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 1500m,
			Currency = "B", // Invalid length
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 2,
			CategoryId = Guid.NewGuid(),
			Period = 2,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Currency");
	}

	[Fact]
	public void Validate_WithEmptyCategoryId_HasErrors()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 1500m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 2,
			CategoryId = Guid.Empty,
			Period = 2,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
	}

	[Fact]
	public void Validate_WithInvalidTransactionType_HasErrors()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 1500m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 5, // Invalid
			CategoryId = Guid.NewGuid(),
			Period = 2,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Type");
	}

	[Fact]
	public void Validate_WithEmptyPeriod_HasErrors()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 1500m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 2,
			CategoryId = Guid.NewGuid(),
			Period = 0, // Invalid
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Period");
	}

	[Fact]
	public void Validate_WithEndDateBeforeTransactionDate_HasErrors()
	{
		// Arrange
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 1500m,
			Currency = "BRL",
			Date = date,
			Type = 2,
			CategoryId = Guid.NewGuid(),
			Period = 2,
			EndDate = date.AddDays(-1) // Before transaction date
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
	}

	[Fact]
	public void Validate_WithEndDateEqualToTransactionDate_HasErrors()
	{
		// Arrange
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 1500m,
			Currency = "BRL",
			Date = date,
			Type = 2,
			CategoryId = Guid.NewGuid(),
			Period = 2,
			EndDate = date // Equal to transaction date
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public void Validate_WithValidPeriods_IsValid(int period)
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Rent",
			Amount = 1500m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 2,
			CategoryId = Guid.NewGuid(),
			Period = period,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.True(result.IsValid);
	}
}

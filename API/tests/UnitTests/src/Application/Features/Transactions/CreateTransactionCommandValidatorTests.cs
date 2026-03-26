using PersonalFinanceAPI.Application.Features.Transactions.Commands;

namespace UnitTests.Application.Features.Transactions;

public class CreateTransactionCommandValidatorTests
{
	private readonly CreateTransactionCommandValidator<CreateTransactionCommand> _validator = new();

	[Fact]
	public void Validate_WithValidCommand_HasNoErrors()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Valid Transaction",
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1, // Income
			CategoryId = Guid.NewGuid()
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
		var command = new CreateTransactionCommand
		{
			Title = "",
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Title" && e.ErrorMessage.Contains("required"));
	}

	[Fact]
	public void Validate_WithTitleExceeding256Characters_HasErrors()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = new string('a', 257),
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Title" && e.ErrorMessage.Contains("256"));
	}

	[Fact]
	public void Validate_WithZeroAmount_HasErrors()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test",
			Amount = 0m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Amount");
	}

	[Fact]
	public void Validate_WithNegativeAmount_HasErrors()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test",
			Amount = -50m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Amount");
	}

	[Fact]
	public void Validate_WithEmptyCurrency_HasErrors()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test",
			Amount = 100m,
			Currency = "",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Currency");
	}

	[Fact]
	public void Validate_WithInvalidCurrencyLength_HasErrors()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test",
			Amount = 100m,
			Currency = "BRLT", // 4 characters instead of 3
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Currency" && e.ErrorMessage.Contains("3"));
	}

	[Fact]
	public void Validate_WithEmptyCategoryId_HasErrors()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test",
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.Empty
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
	}

	[Theory]
	[InlineData(0)]
	[InlineData(3)]
	[InlineData(-1)]
	public void Validate_WithInvalidTransactionType_HasErrors(int type)
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test",
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = type,
			CategoryId = Guid.NewGuid()
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Type");
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	public void Validate_WithValidTransactionType_IsValid(int type)
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test",
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = type,
			CategoryId = Guid.NewGuid()
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.True(result.IsValid);
	}
}

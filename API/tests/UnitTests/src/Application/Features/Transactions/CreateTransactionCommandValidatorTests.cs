using PersonalFinanceAPI.Application.Features.Transactions.Commands;
using static MongoDB.Driver.WriteConcern;

namespace UnitTests.Application.Features.Transactions;

public class CreateTransactionCommandValidatorTests
{
	private readonly CreateTransactionCommandValidator _validator = new();

	[Fact]
	public void Validate_WithValidCommand_HasNoErrors()
	{
		// Arrange
		var command = GetCreateTransactionCommand("Valid Transaction");

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.True(result.IsValid);
	}

	[Fact]
	public void Validate_WithEmptyTitle_HasErrors()
	{
		// Arrange
		var command = GetCreateTransactionCommand("");

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
		var command = GetCreateTransactionCommand(new string('a', 257));

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
		var command = GetCreateTransactionCommand("Test", 0m);

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
		var command = GetCreateTransactionCommand("Test", -50m);

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
		var command = GetCreateTransactionCommand(Currency: "");

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
		var command = GetCreateTransactionCommand(Currency: "BRLT");

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
		var command = GetCreateTransactionCommand(CategoryId: Guid.Empty);

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
		var command = GetCreateTransactionCommand(Type: type);

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
		var command = GetCreateTransactionCommand(Type: type);

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.True(result.IsValid);
	}

	protected CreateTransactionCommand GetCreateTransactionCommand(string Title = "Title", decimal Amount = 100m, DateOnly? Date = null, 
		int Type = 1, Guid? CategoryId = null, string Currency = "BRL")
	{
		if(!Date.HasValue)
		{
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		}
		if(CategoryId is null)
		{
			CategoryId = Guid.NewGuid();
		}

		return new CreateTransactionCommand(Title, Amount, Date.Value, Type, CategoryId.Value, Currency);
	}
}

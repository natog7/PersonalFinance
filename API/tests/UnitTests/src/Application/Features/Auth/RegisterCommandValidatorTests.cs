namespace UnitTests.Application.Features.Auth;

public class RegisterCommandValidatorTests
{
	private readonly RegisterCommandValidator _validator = new();

	[Fact]
	public void Validate_WithValidCommand_HasNoErrors()
	{
		// Arrange
		var command = new RegisterCommand
		{
			Email = "test@example.com",
			Password = "SecurePassword123!",
			FullName = "Test User"
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.True(result.IsValid);
	}

	[Fact]
	public void Validate_WithEmptyEmail_HasErrors()
	{
		// Arrange
		var command = new RegisterCommand
		{
			Email = "",
			Password = "SecurePassword123!",
			FullName = "Test User"
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Email");
	}

	[Fact]
	public void Validate_WithInvalidEmail_HasErrors()
	{
		// Arrange
		var command = new RegisterCommand
		{
			Email = "invalid-email",
			Password = "SecurePassword123!",
			FullName = "Test User"
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Email");
	}

	[Fact]
	public void Validate_WithEmptyPassword_HasErrors()
	{
		// Arrange
		var command = new RegisterCommand
		{
			Email = "test@example.com",
			Password = "",
			FullName = "Test User"
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Password");
	}

	[Fact]
	public void Validate_WithShortPassword_HasErrors()
	{
		// Arrange
		var command = new RegisterCommand
		{
			Email = "test@example.com",
			Password = "short",
			FullName = "Test User"
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "Password");
	}

	[Fact]
	public void Validate_WithEmptyFullName_HasErrors()
	{
		// Arrange
		var command = new RegisterCommand
		{
			Email = "test@example.com",
			Password = "SecurePassword123!",
			FullName = ""
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "FullName");
	}

	[Fact]
	public void Validate_WithFullNameTooLong_HasErrors()
	{
		// Arrange
		var command = new RegisterCommand
		{
			Email = "test@example.com",
			Password = "SecurePassword123!",
			FullName = new string('a', 256)
		};

		// Act
		var result = _validator.Validate(command);

		// Assert
		Assert.False(result.IsValid);
		Assert.Contains(result.Errors, e => e.PropertyName == "FullName");
	}
}

//using PersonalFinanceAPI.Infrastructure.Security;

namespace UnitTests.Domain;

public class UserTests
{
	[Fact]
	public void Create_WithValidData_ReturnsUserInstance()
	{
		// Act
		var user = User.Create("test@example.com", "hashed_password", "Test User");

		// Assert
		Assert.NotEqual(Guid.Empty, user.Id);
		Assert.Equal("test@example.com", user.Email);
		Assert.Equal("hashed_password", user.PasswordHash);
		Assert.Equal("Test User", user.Nickname);
		Assert.True(user.IsActive);
		Assert.Equal(UserRole.User, user.Role);
		Assert.Equal(DateTime.UtcNow.Date, user.CreatedAt.Date);
	}

	[Fact]
	public void Create_WithEmptyEmail_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => User.Create("", "password", "Name"));
		Assert.Contains("Email is required", ex.Message);
	}

	[Fact]
	public void Create_WithWhitespaceEmail_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => User.Create("   ", "password", "Name"));
		Assert.Contains("Email is required", ex.Message);
	}

	[Fact]
	public void Create_WithEmptyPassword_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => User.Create("test@example.com", "", "Name"));
		Assert.Contains("Password is required", ex.Message);
	}

	[Fact]
	public void Create_WithEmptyNickname_ThrowsArgumentException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentException>(() => User.Create("test@example.com", "password", ""));
		Assert.Contains("Full name is required", ex.Message);
	}

	[Fact]
	public void Create_NormalizesEmailToLowercase()
	{
		// Act
		var user = User.Create("Test@Example.COM", "password", "Test User");

		// Assert
		Assert.Equal("test@example.com", user.Email);
	}

	[Fact]
	public void Create_TrimsEmailAndNickname()
	{
		// Act
		var user = User.Create("  test@example.com  ", "password", "  Test User  ");

		// Assert
		Assert.Equal("test@example.com", user.Email);
		Assert.Equal("Test User", user.Nickname);
	}

	[Fact]
	public void UpdateLastLogin_SetsLastLoginAtToCurrentTime()
	{
		// Arrange
		var user = User.Create("test@example.com", "password", "Test User");

		// Act
		user.UpdateLastLogin();

		// Assert
		Assert.NotNull(user.LastLoginAt);
		Assert.Equal(DateTime.UtcNow.Date, user.LastLoginAt.Value.Date);
	}

	[Fact]
	public void IsActive_CanBeSetToFalse()
	{
		// Arrange
		var user = User.Create("test@example.com", "password", "Test User");

		// Act
		user.IsActive = false;

		// Assert
		Assert.False(user.IsActive);
	}

	[Fact]
	public void Role_CanBeChanged()
	{
		// Arrange
		var user = User.Create("test@example.com", "password", "Test User");

		// Act
		user.Role = UserRole.Admin;

		// Assert
		Assert.Equal(UserRole.Admin, user.Role);
	}

	[Fact]
	public void HashPasswordVerify_ReturnsEqualsAndTrue()
	{
		// Arrange
		string password = "password";
		var passwordHasherMock = new Mock<IPasswordHasher>();
		string passwordHash = "hashed_password";

		passwordHasherMock.Setup(h => h.HashPassword(password)).Returns(passwordHash);
		passwordHasherMock.Setup(h => h.VerifyPassword(password, passwordHash)).Returns(true);

		//BcryptPasswordHasher hasher = new BcryptPasswordHasher();
		//passwordHash = hasher.HashPassword(password);

		// Act
		var hashed = passwordHasherMock.Object.HashPassword(password);
		var user = User.Create("test@example.com", hashed, "Test User");
		var verify = passwordHasherMock.Object.VerifyPassword(password, user.PasswordHash);

		// Assert
		Assert.Equal(passwordHash, user.PasswordHash);
		Assert.True(verify);
	}
}

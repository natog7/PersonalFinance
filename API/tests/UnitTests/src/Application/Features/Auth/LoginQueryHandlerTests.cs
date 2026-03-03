namespace UnitTests.Application.Features.Auth;

public class LoginQueryHandlerTests
{
	private readonly Mock<IUserRepository> _mockUserRepository;
	private readonly Mock<ITokenService> _mockTokenService;
	private readonly Mock<IPasswordHasher> _mockPasswordHasher;
	private readonly LoginQueryHandler _handler;

	public LoginQueryHandlerTests()
	{
		_mockUserRepository = new Mock<IUserRepository>();
		_mockTokenService = new Mock<ITokenService>();
		_mockPasswordHasher = new Mock<IPasswordHasher>();
		_handler = new LoginQueryHandler(_mockUserRepository.Object, _mockTokenService.Object, _mockPasswordHasher.Object);
	}

	[Fact]
	public async Task Handle_WithValidCredentials_ReturnsLoginResponse()
	{
		// Arrange
		var email = "test@example.com";
		var password = "Password123!";
		var userId = Guid.NewGuid();
		var user = User.Create(email, "hashed_password", "Test User");

		_mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		_mockPasswordHasher.Setup(h => h.VerifyPassword(password, "hashed_password"))
			.Returns(true);

		_mockTokenService.Setup(t => t.GenerateAccessToken(user.Id, email, user.Role))
			.Returns("access_token");

		_mockTokenService.Setup(t => t.GenerateRefreshToken())
			.Returns("refresh_token");

		_mockUserRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var query = new LoginQuery { Email = email, Password = password };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(user.Id, result.UserId);
		Assert.Equal(email, result.Email);
		Assert.Equal("Test User", result.FullName);
		Assert.Equal("access_token", result.Token.AccessToken);
		Assert.Equal("refresh_token", result.Token.RefreshToken);
	}

	[Fact]
	public async Task Handle_WithInvalidPassword_ReturnsNull()
	{
		// Arrange
		var email = "test@example.com";
		var password = "WrongPassword";
		var user = User.Create(email, "hashed_password", "Test User");

		_mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		_mockPasswordHasher.Setup(h => h.VerifyPassword(password, "hashed_password"))
			.Returns(false);

		var query = new LoginQuery { Email = email, Password = password };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task Handle_WithNonExistentUser_ReturnsNull()
	{
		// Arrange
		var email = "nonexistent@example.com";
		var password = "Password123!";

		_mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);

		var query = new LoginQuery { Email = email, Password = password };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task Handle_WithInactiveUser_ReturnsNull()
	{
		// Arrange
		var email = "test@example.com";
		var password = "Password123!";
		var user = User.Create(email, "hashed_password", "Test User");
		user.IsActive = false;

		_mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		var query = new LoginQuery { Email = email, Password = password };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task Handle_WithEmptyEmail_ReturnsNull()
	{
		// Arrange
		var query = new LoginQuery { Email = "", Password = "Password123!" };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Null(result);
		_mockUserRepository.Verify(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task Handle_WithEmptyPassword_ReturnsNull()
	{
		// Arrange
		var query = new LoginQuery { Email = "test@example.com", Password = "" };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Null(result);
		_mockUserRepository.Verify(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task Handle_UpdatesUserLastLogin()
	{
		// Arrange
		var email = "test@example.com";
		var password = "Password123!";
		var user = User.Create(email, "hashed_password", "Test User");

		_mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(user);

		_mockPasswordHasher.Setup(h => h.VerifyPassword(password, "hashed_password"))
			.Returns(true);

		_mockTokenService.Setup(t => t.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>()))
			.Returns("access_token");

		_mockTokenService.Setup(t => t.GenerateRefreshToken())
			.Returns("refresh_token");

		_mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var query = new LoginQuery { Email = email, Password = password };

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		_mockUserRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => 
			new LoginQueryHandler(null!, _mockTokenService.Object, _mockPasswordHasher.Object));
		Assert.Equal("userRepository", ex.ParamName);
	}

	[Fact]
	public void Constructor_WithNullTokenService_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => 
			new LoginQueryHandler(_mockUserRepository.Object, null!, _mockPasswordHasher.Object));
		Assert.Equal("tokenService", ex.ParamName);
	}

	[Fact]
	public void Constructor_WithNullPasswordHasher_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => 
			new LoginQueryHandler(_mockUserRepository.Object, _mockTokenService.Object, null!));
		Assert.Equal("passwordHasher", ex.ParamName);
	}
}

namespace UnitTests.Application.Features.Auth;

public class RegisterCommandHandlerTests
{
	private readonly Mock<IUserRepository> _mockUserRepository;
	private readonly Mock<IPasswordHasher> _mockPasswordHasher;
	private readonly RegisterCommandHandler _handler;

	public RegisterCommandHandlerTests()
	{
		_mockUserRepository = new Mock<IUserRepository>();
		_mockPasswordHasher = new Mock<IPasswordHasher>();
		_handler = new RegisterCommandHandler(_mockUserRepository.Object, _mockPasswordHasher.Object);
	}

	[Fact]
	public async Task Handle_WithValidCommand_ReturnsRegisterResponse()
	{
		// Arrange
		var command = new PersonalFinanceAPI.Application.Features.Auth.RegisterCommand
		{
			Email = "test@example.com",
			Password = "Password123!",
			FullName = "Test User"
		};

		_mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_mockPasswordHasher.Setup(h => h.HashPassword(command.Password))
			.Returns("hashed_password");

		_mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("test@example.com", result.Email);
		Assert.Equal("Test User", result.FullName);
		Assert.NotEqual(Guid.Empty, result.UserId);
	}

	[Fact]
	public async Task Handle_WithExistingEmail_ReturnsNull()
	{
		// Arrange
		var command = new PersonalFinanceAPI.Application.Features.Auth.RegisterCommand
		{
			Email = "existing@example.com",
			Password = "Password123!",
			FullName = "Test User"
		};

		_mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Null(result);
		_mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task Handle_ChecksIfEmailExists()
	{
		// Arrange
		var command = new PersonalFinanceAPI.Application.Features.Auth.RegisterCommand
		{
			Email = "test@example.com",
			Password = "Password123!",
			FullName = "Test User"
		};

		_mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_mockPasswordHasher.Setup(h => h.HashPassword(command.Password))
			.Returns("hashed_password");

		_mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_mockUserRepository.Verify(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Handle_HashesPassword()
	{
		// Arrange
		var password = "MySecurePassword123!";
		var command = new PersonalFinanceAPI.Application.Features.Auth.RegisterCommand
		{
			Email = "test@example.com",
			Password = password,
			FullName = "Test User"
		};

		_mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_mockPasswordHasher.Setup(h => h.HashPassword(password))
			.Returns("hashed_password");

		_mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_mockPasswordHasher.Verify(h => h.HashPassword(password), Times.Once);
	}

	[Fact]
	public async Task Handle_AddsUserToRepository()
	{
		// Arrange
		var command = new PersonalFinanceAPI.Application.Features.Auth.RegisterCommand
		{
			Email = "test@example.com",
			Password = "Password123!",
			FullName = "Test User"
		};

		_mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_mockPasswordHasher.Setup(h => h.HashPassword(command.Password))
			.Returns("hashed_password");

		_mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => 
			new RegisterCommandHandler(null!, _mockPasswordHasher.Object));
		Assert.Equal("userRepository", ex.ParamName);
	}

	[Fact]
	public void Constructor_WithNullPasswordHasher_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => 
			new RegisterCommandHandler(_mockUserRepository.Object, null!));
		Assert.Equal("passwordHasher", ex.ParamName);
	}
}

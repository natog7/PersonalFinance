namespace UnitTests.Application.Features.Transactions;

public class CreateTransactionCommandHandlerTests
{
	private readonly Mock<ITransactionRepository> _mockRepository;
	private readonly CreateTransactionCommandHandler _handler;

	public CreateTransactionCommandHandlerTests()
	{
		_mockRepository = new Mock<ITransactionRepository>();
		_handler = new CreateTransactionCommandHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Handle_WithValidCommand_ReturnsTransactionId()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test Transaction",
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		_mockRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.NotEqual(Guid.Empty, result.Id);
		_mockRepository.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Handle_CallsRepositoryAddAsync()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test Transaction",
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		_mockRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_mockRepository.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Handle_CreatesTransactionWithCorrectValues()
	{
		// Arrange
		var title = "Test Transaction";
		var amount = 100m;
		var currency = "BRL";
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var type = 1;
		var categoryId = Guid.NewGuid();

		var command = new CreateTransactionCommand
		{
			Title = title,
			Amount = amount,
			Currency = currency,
			Date = date,
			Type = type,
			CategoryId = categoryId
		};

		Transaction? capturedTransaction = null;
		_mockRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
			.Callback<Transaction, CancellationToken>((t, _) => capturedTransaction = t)
			.Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.NotNull(capturedTransaction);
		Assert.Equal(title, capturedTransaction.Title);
		Assert.Equal(amount, capturedTransaction.Amount.Amount);
		Assert.Equal(currency, capturedTransaction.Amount.Currency);
		Assert.Equal(date, capturedTransaction.Date);
		Assert.Equal((TransactionType)type, capturedTransaction.Type);
		Assert.Equal(categoryId, capturedTransaction.CategoryId);
	}

	[Fact]
	public void Constructor_WithNullRepository_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => new CreateTransactionCommandHandler(null!));
		Assert.Equal("repository", ex.ParamName);
	}

	[Fact]
	public async Task Handle_WithRepositoryException_ThrowsException()
	{
		// Arrange
		var command = new CreateTransactionCommand
		{
			Title = "Test Transaction",
			Amount = 100m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid()
		};

		_mockRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Database error"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
	}
}

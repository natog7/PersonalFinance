namespace UnitTests.Application.Features.RecurrentTransactions;

public class CreateRecurrentTransactionCommandHandlerTests
{
	private readonly Mock<ITransactionRepository> _mockRepository;
	private readonly CreateRecurrentTransactionCommandHandler _handler;

	public CreateRecurrentTransactionCommandHandlerTests()
	{
		_mockRepository = new Mock<ITransactionRepository>();
		_handler = new CreateRecurrentTransactionCommandHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Handle_WithValidCommand_ReturnsTransactionId()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Salary",
			Amount = 5000m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1, // Income
			CategoryId = Guid.NewGuid(),
			Period = 2, // Monthly
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		_mockRepository.Setup(r => r.AddAsync(It.IsAny<RecurrentTransaction>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.NotEqual(Guid.Empty, result.Id);
	}

	[Fact]
	public async Task Handle_CallsRepositoryAddAsync()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Salary",
			Amount = 5000m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid(),
			Period = 2,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		_mockRepository.Setup(r => r.AddAsync(It.IsAny<RecurrentTransaction>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_mockRepository.Verify(r => r.AddAsync(It.IsAny<RecurrentTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Handle_CreatesRecurrentTransactionWithCorrectValues()
	{
		// Arrange
		var title = "Monthly Salary";
		var amount = 5000m;
		var currency = "BRL";
		var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
		var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12));
		var type = 1;
		var categoryId = Guid.NewGuid();
		var period = 2;

		var command = new CreateRecurrentTransactionCommand
		{
			Title = title,
			Amount = amount,
			Currency = currency,
			Date = date,
			Type = type,
			CategoryId = categoryId,
			Period = period,
			EndDate = endDate
		};

		RecurrentTransaction? capturedTransaction = null;
		_mockRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
			.Callback<Transaction, CancellationToken>((t, _) => capturedTransaction = (RecurrentTransaction)t)
			.Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.NotNull(capturedTransaction);
		Assert.Equal(title, capturedTransaction.Title);
		Assert.Equal(amount, capturedTransaction.Amount.Amount);
		Assert.Equal(currency, capturedTransaction.Amount.Currency);
		Assert.Equal(date, capturedTransaction.Date);
		Assert.Equal(endDate, capturedTransaction.EndDate);
		Assert.Equal((TransactionType)type, capturedTransaction.Type);
		Assert.Equal(categoryId, capturedTransaction.CategoryId);
		Assert.Equal((RecurrentPeriod)period, capturedTransaction.Period);
	}

	[Fact]
	public void Constructor_WithNullRepository_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => new CreateRecurrentTransactionCommandHandler(null!));
		Assert.Equal("repository", ex.ParamName);
	}

	[Fact]
	public async Task Handle_WithRepositoryException_ThrowsException()
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Monthly Salary",
			Amount = 5000m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 1,
			CategoryId = Guid.NewGuid(),
			Period = 2,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		_mockRepository.Setup(r => r.AddAsync(It.IsAny<RecurrentTransaction>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Database error"));

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task Handle_WithDifferentPeriods_CreatesCorrectRecurrentTransaction(int period)
	{
		// Arrange
		var command = new CreateRecurrentTransactionCommand
		{
			Title = "Test",
			Amount = 1000m,
			Currency = "BRL",
			Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			Type = 2,
			CategoryId = Guid.NewGuid(),
			Period = period,
			EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
		};

		RecurrentTransaction? capturedTransaction = null;
		_mockRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
			.Callback<Transaction, CancellationToken>((t, _) => capturedTransaction = (RecurrentTransaction)t)
			.Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.NotNull(capturedTransaction);
		Assert.Equal((RecurrentPeriod)period, capturedTransaction.Period);
	}
}

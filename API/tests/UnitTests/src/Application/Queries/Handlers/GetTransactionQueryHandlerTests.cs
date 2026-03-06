namespace UnitTests.Application.Queries.Handlers;

public class GetTransactionQueryHandlerTests
{
	private readonly Mock<ITransactionRepository> _mockRepository;
	private readonly GetTransactionQueryHandler _handler;

	public GetTransactionQueryHandlerTests()
	{
		_mockRepository = new Mock<ITransactionRepository>();
		_handler = new GetTransactionQueryHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Handle_WithExistingTransaction_ReturnsTransactionDto()
	{
		// Arrange
		var transactionId = Guid.NewGuid();
		var category = Category.Create("Test Category");
		var transaction = Transaction.Create("Test Transaction", Money.Create(100m, "BRL"),
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Income, category.Id);
		

		var prop = typeof(Transaction).GetProperty("Category");
		prop?.SetValue(transaction, category);

		// Setup mock to return the transaction with category
		_mockRepository.Setup(r => r.GetByIdAsync(transactionId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(transaction);

		var query = new GetTransactionQuery { Id = transactionId };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(transaction.Id, result.Id);
		Assert.Equal("Test Transaction", result.Title);
		Assert.Equal(100m, result.Amount);
		Assert.Equal("BRL", result.Currency);
		Assert.Equal((int)TransactionType.Income, result.Type);
	}

	[Fact]
	public async Task Handle_WithNonExistingTransaction_ReturnsNull()
	{
		// Arrange
		var transactionId = Guid.NewGuid();

		_mockRepository.Setup(r => r.GetByIdAsync(transactionId, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Transaction?)null);

		var query = new GetTransactionQuery { Id = transactionId };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task Handle_CallsRepositoryGetByIdAsync()
	{
		// Arrange
		var transactionId = Guid.NewGuid();

		_mockRepository.Setup(r => r.GetByIdAsync(transactionId, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Transaction?)null);

		var query = new GetTransactionQuery { Id = transactionId };

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		_mockRepository.Verify(r => r.GetByIdAsync(transactionId, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public void Constructor_WithNullRepository_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => new GetTransactionQueryHandler(null!));
		Assert.Equal("repository", ex.ParamName);
	}

	[Fact]
	public async Task Handle_MapsTransactionToDto()
	{
		// Arrange
		var category = Category.Create("Category");
		var transaction = Transaction.Create("Salary", Money.Create(5000m, "BRL"),
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Income, category.Id);

		var prop = typeof(Transaction).GetProperty("Category");
		prop?.SetValue(transaction, category);

		_mockRepository.Setup(r => r.GetByIdAsync(transaction.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(transaction);

		var query = new GetTransactionQuery { Id = transaction.Id };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(transaction.Id, result.Id);
		Assert.Equal("Salary", result.Title);
		Assert.Equal(5000m, result.Amount);
		Assert.Equal("BRL", result.Currency);
		Assert.Equal(category.Id, result.CategoryId);
		Assert.Equal((int)TransactionType.Income, result.Type);
		Assert.False(result.IsRecurrent);
	}

	[Fact]
	public async Task Handle_WithRecurrentTransaction_SetsIsRecurrentTrue()
	{
		// Arrange
		var category = Category.Create("Category");
		var transaction = RecurrentTransaction.Create("Monthly Rent", Money.Create(1500m, "BRL"),
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
			DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12)),
			TransactionType.Expense, category.Id, RecurrentPeriod.Monthly);

		var prop = typeof(Transaction).GetProperty("Category");
		prop?.SetValue(transaction, category);

		_mockRepository.Setup(r => r.GetByIdAsync(transaction.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(transaction);

		var query = new GetTransactionQuery { Id = transaction.Id };

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsRecurrent);
	}
}

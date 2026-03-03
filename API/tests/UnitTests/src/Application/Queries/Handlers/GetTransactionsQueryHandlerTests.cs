namespace UnitTests.Application.Queries.Handlers;

public class GetTransactionsQueryHandlerTests
{
	private readonly Mock<ITransactionRepository> _mockRepository;
	private readonly GetTransactionsQueryHandler _handler;

	public GetTransactionsQueryHandlerTests()
	{
		_mockRepository = new Mock<ITransactionRepository>();
		_handler = new GetTransactionsQueryHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Handle_WithTransactions_ReturnsTransactionDtos()
	{
		// Arrange
		var categoryId1 = Guid.NewGuid();
		var categoryId2 = Guid.NewGuid();
		
		var transactions = new List<Transaction>
		{
			Transaction.Create("Salary", Money.Create(5000m, "BRL"),
				DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Income, categoryId1),
			Transaction.Create("Grocery", Money.Create(150m, "BRL"),
				DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)), TransactionType.Expense, categoryId2)
		};

		_mockRepository.Setup(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(transactions);

		var query = new GetTransactionsQuery();

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.NotEmpty(result.Transactions);
		Assert.Equal(2, result.Transactions.Count);
	}

	[Fact]
	public async Task Handle_WithNoTransactions_ReturnsEmptyList()
	{
		// Arrange
		_mockRepository.Setup(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Transaction>());

		var query = new GetTransactionsQuery();

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result.Transactions);
	}

	[Fact]
	public async Task Handle_CallsRepositoryGetFilterAsync()
	{
		// Arrange
		_mockRepository.Setup(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Transaction>());

		var query = new GetTransactionsQuery();

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		_mockRepository.Verify(r => r.GetFilterAsync(query, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public void Constructor_WithNullRepository_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => new GetTransactionsQueryHandler(null!));
		Assert.Equal("repository", ex.ParamName);
	}

	[Fact]
	public async Task Handle_MapsTransactionsToDto()
	{
		// Arrange
		var categoryId = Guid.NewGuid();
		var transaction = Transaction.Create("Salary", Money.Create(5000m, "BRL"),
			DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), TransactionType.Income, categoryId);

		_mockRepository.Setup(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Transaction> { transaction });

		var query = new GetTransactionsQuery();

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Single(result.Transactions);
		var dto = result.Transactions[0];
		Assert.Equal(transaction.Id, dto.Id);
		Assert.Equal("Salary", dto.Title);
		Assert.Equal(5000m, dto.Amount);
		Assert.Equal("BRL", dto.Currency);
		Assert.Equal(categoryId, dto.CategoryId);
		Assert.Equal((int)TransactionType.Income, dto.Type);
	}

	[Fact]
	public async Task Handle_WithMultipleTransactions_ReturnsAllAsDtos()
	{
		// Arrange
		var transactions = Enumerable.Range(1, 5)
			.Select(i => Transaction.Create($"Transaction {i}", Money.Create(100m * i, "BRL"),
				DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-i)),
				i % 2 == 0 ? TransactionType.Income : TransactionType.Expense,
				Guid.NewGuid()))
			.ToList();

		_mockRepository.Setup(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(transactions);

		var query = new GetTransactionsQuery();

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.Equal(5, result.Transactions.Count);
		for (int i = 0; i < transactions.Count; i++)
		{
			Assert.Equal(transactions[i].Id, result.Transactions[i].Id);
			Assert.Equal(transactions[i].Title, result.Transactions[i].Title);
		}
	}
}

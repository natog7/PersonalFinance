using PersonalFinanceAPI.Application.Features.Finance;
using PersonalFinanceAPI.Application.Features.Transactions.Queries;

namespace UnitTests.Application.Queries.Handlers;

public class GetBalanceProjectionQueryHandlerTests
{
	private readonly Mock<ITransactionRepository> _mockRepository;
	private readonly GetBalanceProjectionQueryHandler _handler;

	public GetBalanceProjectionQueryHandlerTests()
	{
		_mockRepository = new Mock<ITransactionRepository>();
		_handler = new GetBalanceProjectionQueryHandler(_mockRepository.Object);
	}

	[Fact]
	public async Task Handle_WithValidQuery_ReturnsBalanceProjectionResult()
	{
		// Arrange
		var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
		var query = new GetBalanceProjectionQuery
		{
			StartDate = startDate,
			MonthCount = 3
		};

		_mockRepository.Setup(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Transaction>());

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
	}

	[Fact]
	public async Task Handle_WithZeroMonthCount_ThrowsArgumentException()
	{
		// Arrange
		var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
		var query = new GetBalanceProjectionQuery
		{
			StartDate = startDate,
			MonthCount = 0
		};

		// Act & Assert
		var ex = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
		Assert.Contains("Month count must be greater than zero", ex.Message);
	}

	[Fact]
	public async Task Handle_WithNegativeMonthCount_ThrowsArgumentException()
	{
		// Arrange
		var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
		var query = new GetBalanceProjectionQuery
		{
			StartDate = startDate,
			MonthCount = -5
		};

		// Act & Assert
		var ex = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
		Assert.Contains("Month count must be greater than zero", ex.Message);
	}

	[Fact]
	public async Task Handle_CallsRepositoryGetFilterAsync()
	{
		// Arrange
		var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
		var query = new GetBalanceProjectionQuery
		{
			StartDate = startDate,
			MonthCount = 1
		};

		_mockRepository.Setup(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Transaction>());

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		_mockRepository.Verify(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()), 
			Times.AtLeastOnce);
	}

	[Fact]
	public void Constructor_WithNullRepository_ThrowsArgumentNullException()
	{
		// Act & Assert
		var ex = Assert.Throws<ArgumentNullException>(() => new GetBalanceProjectionQueryHandler(null!));
		Assert.Equal("repository", ex.ParamName);
	}

	[Fact]
	public async Task Handle_WithMultipleMonths_ProcessesAllMonths()
	{
		// Arrange
		var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
		var query = new GetBalanceProjectionQuery
		{
			StartDate = startDate,
			MonthCount = 3
		};

		_mockRepository.Setup(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Transaction>());

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		_mockRepository.Verify(r => r.GetFilterAsync(It.IsAny<GetTransactionsQuery>(), It.IsAny<CancellationToken>()), 
			Times.Exactly(3));
	}
}

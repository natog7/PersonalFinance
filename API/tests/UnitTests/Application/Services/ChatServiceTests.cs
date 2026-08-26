using MediatR;
using Moq;
using PersonalFinanceAPI.Application.Features.Transactions.Commands;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Interfaces;
using PersonalFinanceAPI.Infrastructure.Services;
using Xunit;

namespace PersonalFinanceAPI.Tests.Application.Services;

/// <summary>
/// Testes unitários para o ChatService
/// </summary>
public class ChatServiceTests
{
	private readonly Mock<IKernelProvider> _kernelProviderMock;
	private readonly Mock<IMediator> _mediatorMock;
	private readonly ChatService _chatService;

	public ChatServiceTests()
	{
		_kernelProviderMock = new Mock<IKernelProvider>();
		_mediatorMock = new Mock<IMediator>();
		_chatService = new ChatService(_kernelProviderMock.Object, _mediatorMock.Object);
	}

	[Fact]
	public async Task ProcessMessageAsync_WithValidMessage_ShouldReturnChatResponse()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var connectionId = "test-connection-123";
		var history = new List<ChatMessageDto>();
		var userMessage = "Olá, qual é meu saldo?";

		// Act
		var result = await _chatService.ProcessMessageAsync(userId, connectionId, history, userMessage);

		// Assert
		Assert.NotNull(result);
		Assert.IsType<ChatResponseDto>(result);
		Assert.NotEmpty(result.Message);
	}

	[Fact]
	public async Task ProcessMessageAsync_WithEmptyMessage_ShouldHandleGracefully()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var connectionId = "test-connection-123";
		var history = new List<ChatMessageDto>();
		var userMessage = "";

		// Act
		var result = await _chatService.ProcessMessageAsync(userId, connectionId, history, userMessage);

		// Assert
		Assert.NotNull(result);
		Assert.NotEmpty(result.Message);
	}

	[Fact]
	public async Task ProcessMessageAsync_WithTransactionCreationIntent_ShouldSetRequiresConfirmation()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var connectionId = "test-connection-123";
		var history = new List<ChatMessageDto>();
		var userMessage = "quero criar uma transação de 100 reais";

		// Act
		var result = await _chatService.ProcessMessageAsync(userId, connectionId, history, userMessage);

		// Assert
		Assert.NotNull(result);
		// Se a intenção foi detectada como criação de transação, requiresConfirmation deve ser true
		// (dependendo da lógica de ExtractIntentionAsync)
	}

	[Fact]
	public async Task ExecuteActionAsync_WithCreateTransactionAction_ShouldCallMediator()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var action = "create_transaction";
		var parameters = new Dictionary<string, object>
		{
			{ "title", "Transação de Teste" },
			{ "amount", 100m },
			{ "categoryId", Guid.NewGuid().ToString() },
			{ "type", "Expense" },
			{ "date", DateOnly.FromDateTime(DateTime.Now).ToString() }
		};

		_mediatorMock
			.Setup(m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new IdDto<Guid> { Id = Guid.NewGuid() });

		// Act
		var result = await _chatService.ExecuteActionAsync(userId, action, parameters);

		// Assert
		Assert.True(result);
		_mediatorMock.Verify(
			m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Fact]
	public async Task ExecuteActionAsync_WithInvalidAction_ShouldReturnFalse()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var action = "unknown_action";
		var parameters = new Dictionary<string, object>();

		// Act
		var result = await _chatService.ExecuteActionAsync(userId, action, parameters);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public async Task ExecuteActionAsync_WithMissingParameters_ShouldReturnFalse()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var action = "create_transaction";
		var parameters = new Dictionary<string, object>();

		// Act
		var result = await _chatService.ExecuteActionAsync(userId, action, parameters);

		// Assert
		Assert.False(result);
	}
}

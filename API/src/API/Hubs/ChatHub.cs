using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PersonalFinanceAPI.Application.Features.Chat;
using PersonalFinanceAPI.Application.Features.Chat.Services;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.API.Hubs;

/// <summary>
/// Hub SignalR para comunicação em tempo real com o chat AI
/// Mantém memória efêmera de conversas por conexão
/// </summary>
[Authorize]
public class ChatHub : Hub
{
	private readonly IChatService _chatService;
	private readonly ICurrentUserService _userService;
	private readonly ILogger<ChatHub> _logger;

	// Memória efêmera: connectionId -> histórico de mensagens
	private static readonly Dictionary<string, List<ChatMessageDto>> ConversationHistory = new();

	// Limite de mensagens mantidas em memória por conversa (últimas N mensagens)
	private const int MaxMessagesPerConversation = 50;

	public ChatHub(IChatService chatService, ICurrentUserService userService, ILogger<ChatHub> logger)
	{
		_chatService = chatService;
		_userService = userService;
		_logger = logger;
	}

	/// <summary>
	/// Método chamado quando um cliente conecta ao hub
	/// </summary>
	public override async Task OnConnectedAsync()
	{
		var connectionId = Context.ConnectionId;
		_logger.LogInformation("Cliente conectado ao ChatHub: {ConnectionId}", connectionId);

		// Inicializar histórico vazio para esta conexão
		if (!ConversationHistory.ContainsKey(connectionId))
		{
			ConversationHistory[connectionId] = new List<ChatMessageDto>();
		}

		// Notificar que o usuário conectou
		await Clients.Caller.SendAsync("Connected", new
		{
			message = "Conectado ao assistente financeiro",
			timestamp = DateTime.UtcNow
		});

		await base.OnConnectedAsync();
	}

	/// <summary>
	/// Método chamado quando um cliente desconecta do hub
	/// </summary>
	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		var connectionId = Context.ConnectionId;
		_logger.LogInformation("Cliente desconectado do ChatHub: {ConnectionId}", connectionId);

		// Limpar histórico da conversa (memória efêmera)
		lock (ConversationHistory)
		{
			ConversationHistory.Remove(connectionId);
		}

		if (exception != null)
		{
			_logger.LogError(exception, "Erro na desconexão do ChatHub: {ConnectionId}", connectionId);
		}

		await base.OnDisconnectedAsync(exception);
	}

	/// <summary>
	/// Recebe uma mensagem do cliente e processa via Semantic Kernel
	/// </summary>
	/// <param name="message">Mensagem do usuário</param>
	/// <param name="ct">Token de cancelamento</param>
	[HubMethodName("SendMessage")]
	public async Task SendMessageAsync(string message, CancellationToken ct = default)
	{
		if (string.IsNullOrWhiteSpace(message))
		{
			await Clients.Caller.SendAsync("Error", "Mensagem não pode estar vazia");
			return;
		}

		var connectionId = Context.ConnectionId;
		var userId = _userService.UserId;

		try
		{
			_logger.LogInformation("Mensagem recebida - ConnectionId: {ConnectionId}, UserId: {UserId}", connectionId, userId);

			// Obter histórico da conversa
			List<ChatMessageDto> history;
			lock (ConversationHistory)
			{
				if (!ConversationHistory.TryGetValue(connectionId, out var existingHistory))
				{
					existingHistory = new List<ChatMessageDto>();
					ConversationHistory[connectionId] = existingHistory;
				}
				history = new List<ChatMessageDto>(existingHistory);
			}

			// Adicionar mensagem do usuário ao histórico
			var userMessageDto = new ChatMessageDto
			{
				Role = ChatRole.User,
				Content = message
			};
			history.Add(userMessageDto);

			// Notificar que está processando (typing indicator)
			await Clients.Caller.SendAsync("TypingIndicator", true);

			// Processar mensagem via Semantic Kernel
			var response = await _chatService.ProcessMessageAsync(
				userId,
				connectionId,
				history,
				message,
				ct
			);

			// Adicionar resposta ao histórico
			var assistantMessageDto = new ChatMessageDto
			{
				Role = ChatRole.Assistant,
				Content = response.Message
			};
			history.Add(assistantMessageDto);

			// Manter limite de histórico em memória
			lock (ConversationHistory)
			{
				if (history.Count > MaxMessagesPerConversation)
				{
					// Remover as primeiras mensagens
					var removeCount = history.Count - MaxMessagesPerConversation;
					ConversationHistory[connectionId] = history.Skip(removeCount).ToList();
				}
				else
				{
					ConversationHistory[connectionId] = history;
				}
			}

			// Enviar resposta para o cliente
			await Clients.Caller.SendAsync("ReceiveMessage", new
			{
				message = response.Message,
				suggestion = response.Suggestion,
				requiresConfirmation = response.RequiresConfirmation,
				timestamp = response.Timestamp
			});

			// Parar de mostrar typing indicator
			await Clients.Caller.SendAsync("TypingIndicator", false);

		}
		catch (OperationCanceledException)
		{
			_logger.LogWarning("Operação cancelada - ConnectionId: {ConnectionId}", connectionId);
			await Clients.Caller.SendAsync("Error", "Requisição cancelada. Tente novamente.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao processar mensagem - ConnectionId: {ConnectionId}", connectionId);
			await Clients.Caller.SendAsync("Error", "Erro ao processar sua mensagem. Tente novamente.");
		}
	}

	/// <summary>
	/// Cliente confirma uma ação sugerida
	/// </summary>
	/// <param name="action">Tipo de ação a executar</param>
	/// <param name="parameters">Parâmetros da ação em JSON</param>
	/// <param name="ct">Token de cancelamento</param>
	[HubMethodName("ConfirmAction")]
	public async Task ConfirmActionAsync(string action, Dictionary<string, object> parameters, CancellationToken ct = default)
	{
		var userId = _userService.UserId;

		try
		{
			_logger.LogInformation("Ação confirmada - UserId: {UserId}, Action: {Action}", userId, action);

			var success = await _chatService.ExecuteActionAsync(userId, action, parameters, ct);

			await Clients.Caller.SendAsync("ActionResult", new
			{
				action,
				success,
				message = success ? "Ação executada com sucesso" : "Falha ao executar ação",
				timestamp = DateTime.UtcNow
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao confirmar ação - UserId: {UserId}", userId);
			await Clients.Caller.SendAsync("Error", "Erro ao executar ação confirmada.");
		}
	}

	/// <summary>
	/// Limpar histórico da conversa
	/// </summary>
	[HubMethodName("ClearHistory")]
	public async Task ClearHistoryAsync()
	{
		var connectionId = Context.ConnectionId;
		lock (ConversationHistory)
		{
			ConversationHistory[connectionId] = new List<ChatMessageDto>();
		}

		_logger.LogInformation("Histórico limpo - ConnectionId: {ConnectionId}", connectionId);
		await Clients.Caller.SendAsync("HistoryCleared");
	}
}

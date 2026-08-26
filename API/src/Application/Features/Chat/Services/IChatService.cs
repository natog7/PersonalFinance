namespace PersonalFinanceAPI.Application.Features.Chat.Services;

public interface IChatService
{
	/// <summary>
	/// Processa uma mensagem do usuário e retorna uma resposta baseada em IA
	/// </summary>
	/// <param name="userId">ID do usuário autenticado</param>
	/// <param name="connectionId">ID da conexão SignalR (para contexto de conversa)</param>
	/// <param name="history">Histórico de mensagens da conversa</param>
	/// <param name="userMessage">Mensagem do usuário</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>Resposta processada do assistente</returns>
	Task<ChatResponseDto> ProcessMessageAsync(
		Guid? userId,
		string connectionId,
		IReadOnlyList<ChatMessageDto> history,
		string userMessage,
		CancellationToken ct = default);

	/// <summary>
	/// Processa uma ação extraída (ex: criar transação) de forma integrada
	/// </summary>
	/// <param name="userId">ID do usuário</param>
	/// <param name="action">Tipo de ação (ex: "create_transaction")</param>
	/// <param name="parameters">Parâmetros da ação</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>Resultado da ação (true se sucesso)</returns>
	Task<bool> ExecuteActionAsync(
		Guid? userId,
		string action,
		Dictionary<string, object> parameters,
		CancellationToken ct = default);
}

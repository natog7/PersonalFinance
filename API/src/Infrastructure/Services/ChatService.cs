using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using PersonalFinanceAPI.Application.Features.Chat;
using PersonalFinanceAPI.Application.Features.Chat.Services;
using PersonalFinanceAPI.Application.Features.Transactions.Commands;
using PersonalFinanceAPI.Domain.Enums;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Interfaces;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Templates;

namespace PersonalFinanceAPI.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de chat utilizando Semantic Kernel
/// </summary>
public class ChatService : IChatService
{
	private readonly IKernelProvider _kernelProvider;
	private readonly IMediator _mediator;
	private Kernel? _kernel;

	public ChatService(IKernelProvider kernelProvider, IMediator mediator)
	{
		_kernelProvider = kernelProvider;
		_mediator = mediator;
	}

	public async Task<ChatResponseDto> ProcessMessageAsync(
		Guid? userId,
		string connectionId,
		IReadOnlyList<ChatMessageDto> history,
		string userMessage,
		CancellationToken ct = default)
	{
		// Inicializar kernel na primeira chamada
		_kernel ??= await _kernelProvider.CreateKernelAsync();

		// Construir histórico para o LLM
		var chatHistory = new ChatHistory(PromptTemplates.SystemPrompt);

		foreach (var msg in history)
		{
			switch (msg.Role)
			{
				case ChatRole.User:
					chatHistory.AddUserMessage(msg.Content);
					break;
				case ChatRole.Assistant:
					chatHistory.AddAssistantMessage(msg.Content);
					break;
			}
		}

		// Adicionar mensagem atual do usuário
		chatHistory.AddUserMessage(userMessage);

		// Obter serviço de chat completion
		var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

		try
		{
			// Executar chamada ao LLM
			var response = await chatCompletionService.GetChatMessageContentAsync(
				chatHistory,
				cancellationToken: ct
			);

			var assistantMessage = response.Content ?? "Desculpe, não consegui gerar uma resposta.";

			// Tentar extrair intenção/ação da resposta
			var (action, suggestion, requiresConfirmation) = await ExtractIntentionAsync(
				userMessage,
				assistantMessage,
				ct
			);

			return new ChatResponseDto
			{
				Message = assistantMessage,
				Suggestion = suggestion,
				RequiresConfirmation = requiresConfirmation
			};
		}
		catch (Exception ex)
		{
			// Log e resposta de fallback
			Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
			return new ChatResponseDto
			{
				Message = "Desculpe, ocorreu um erro ao processar sua mensagem. Tente novamente.",
				RequiresConfirmation = false
			};
		}
	}

	public async Task<bool> ExecuteActionAsync(Guid? userId, string action, Dictionary<string, object> parameters, CancellationToken ct = default)
	{
		if (!userId.HasValue)
			return false;

		return action.ToLowerInvariant() switch
		{
			"create_transaction" => await ExecuteCreateTransactionAsync(userId.Value, parameters, ct),
			_ => false
		};
	}

	private async Task<bool> ExecuteCreateTransactionAsync(Guid userId, Dictionary<string, object> parameters, CancellationToken ct)
	{
		try
		{
			// Extrair parâmetros
			var title = parameters.ContainsKey("title") ? parameters["title"].ToString() : null;
			var amount = parameters.ContainsKey("amount") ? decimal.Parse(parameters["amount"].ToString()!) : 0m;
			var categoryId = parameters.ContainsKey("categoryId") ? Guid.Parse(parameters["categoryId"].ToString()!) : Guid.Empty;
			var transactionType = parameters.ContainsKey("type") 
				? Enum.Parse<TransactionType>(parameters["type"].ToString()!)
				: TransactionType.Expense;
			var date = parameters.ContainsKey("date") 
				? DateOnly.Parse(parameters["date"].ToString()!)
				: DateOnly.FromDateTime(DateTime.Now);

			// Validar dados mínimos
			if (string.IsNullOrWhiteSpace(title) || amount <= 0 || categoryId == Guid.Empty)
				return false;

			// Criar e enviar comando
			var command = new CreateTransactionCommand(
				Title: title,
				Amount: amount,
				Date: date,
				Type: transactionType,
				CategoryId: categoryId
			);

			await _mediator.Send(command, ct);
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Extrai a intenção/ação da mensagem do usuário
	/// </summary>
	private async Task<(string action, string? suggestion, bool requiresConfirmation)> ExtractIntentionAsync(
		string userMessage,
		string assistantMessage,
		CancellationToken ct)
	{
		// Lógica simples de detecção de padrões (pode ser expandida com SK)
		var lowerMessage = userMessage.ToLowerInvariant();

		if (lowerMessage.Contains("criar") && (lowerMessage.Contains("transação") || lowerMessage.Contains("despesa") || lowerMessage.Contains("receita")))
		{
			return ("create_transaction", "Deseja criar uma transação com esses dados?", true);
		}

		if (lowerMessage.Contains("gastar") || lowerMessage.Contains("gastei"))
		{
			// Tentar extrair valor e categoria
			return ("create_transaction", null, true);
		}

		if (lowerMessage.Contains("saldo") || lowerMessage.Contains("quanto tenho"))
		{
			return ("view_balance", null, false);
		}

		return ("none", null, false);
	}
}

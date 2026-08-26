namespace PersonalFinanceAPI.Infrastructure.SemanticKernel.Models;

/// <summary>
/// Configuração de provedor LLM (OpenAI, Gemini, Claude)
/// </summary>
public record LLMProviderConfig
{
	public required string Provider { get; init; }
	public required string ApiKey { get; init; }
	public string? BaseUrl { get; init; }
	public string? ModelId { get; init; }
	public int MaxTokens { get; init; } = 2000;
	public decimal Temperature { get; init; } = 0.7m;
}

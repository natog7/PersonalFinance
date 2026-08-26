using PersonalFinanceAPI.Infrastructure.SemanticKernel.Interfaces;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Models;

namespace PersonalFinanceAPI.Infrastructure.SemanticKernel.Factory;

/// <summary>
/// Factory para criar instâncias de IKernelProvider baseado na configuração
/// </summary>
public class KernelProviderFactory
{
	/// <summary>
	/// Cria um provedor de kernel baseado na configuração fornecida
	/// </summary>
	/// <param name="config">Configuração do LLM provider</param>
	/// <returns>Instância de IKernelProvider apropriada</returns>
	/// <exception cref="InvalidOperationException">Quando o provider não é suportado</exception>
	public static IKernelProvider CreateProvider(LLMProviderConfig config)
	{
		return config.Provider.ToLowerInvariant() switch
		{
			"openai" => new Providers.OpenAIKernelProvider(config),
			"gemini" => new Providers.GeminiKernelProvider(config),
			"google" => new Providers.GeminiKernelProvider(config),
			"claude" => new Providers.ClaudeKernelProvider(config),
			"anthropic" => new Providers.ClaudeKernelProvider(config),
			_ => throw new InvalidOperationException($"Unsupported LLM provider: {config.Provider}")
		};
	}
}

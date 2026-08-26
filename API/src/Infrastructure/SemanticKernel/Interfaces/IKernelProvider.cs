using Microsoft.SemanticKernel;

namespace PersonalFinanceAPI.Infrastructure.SemanticKernel.Interfaces;

/// <summary>
/// Interface para abstrair a criação de instâncias do Kernel Semantic com diferentes provedores LLM
/// </summary>
public interface IKernelProvider
{
	/// <summary>
	/// Cria e retorna uma instância configurada do Kernel
	/// </summary>
	Task<Kernel> CreateKernelAsync();

	/// <summary>
	/// Nome do provedor (e.g., "openai", "gemini", "claude")
	/// </summary>
	string ProviderName { get; }
}

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Interfaces;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Models;

namespace PersonalFinanceAPI.Infrastructure.SemanticKernel.Providers;

/// <summary>
/// Provedor de Kernel para OpenAI (GPT-4, GPT-4 Turbo, GPT-3.5, etc.)
/// </summary>
public class OpenAIKernelProvider : IKernelProvider
{
	private readonly LLMProviderConfig _config;

	public string ProviderName => "openai";

	public OpenAIKernelProvider(LLMProviderConfig config)
	{
		_config = config;
	}

	public async Task<Kernel> CreateKernelAsync()
	{
		var builder = Kernel.CreateBuilder();

		builder.AddOpenAIChatCompletion(
			modelId: _config.ModelId ?? "gpt-4",
			apiKey: _config.ApiKey
		);

		var kernel = builder.Build();

		return await Task.FromResult(kernel);
	}
}

using Anthropic;
using Microsoft.SemanticKernel;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Interfaces;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Models;

namespace PersonalFinanceAPI.Infrastructure.SemanticKernel.Providers;

/// <summary>
/// Provedor de Kernel para Claude (Anthropic)
/// </summary>
public class ClaudeKernelProvider : IKernelProvider
{
	private readonly LLMProviderConfig _config;

	public string ProviderName => "claude";

	public ClaudeKernelProvider(LLMProviderConfig config)
	{
		_config = config;
	}

	public async Task<Kernel> CreateKernelAsync()
	{
		var builder = Kernel.CreateBuilder();

		//builder.AddOpenAIChatCompletion(
		//	modelId: _config.ModelId ?? "claude-3-5-sonnet-20241022",
		//	apiKey: _config.ApiKey,
		//	endpoint: new Uri("https://api.anthropic.com/v1/")
		//);

		var kernel = builder.Build();

		return await Task.FromResult(kernel);
	}
}

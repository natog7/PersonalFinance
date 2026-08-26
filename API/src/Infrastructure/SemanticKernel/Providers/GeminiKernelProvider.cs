using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.Connectors.Google;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Interfaces;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Models;

namespace PersonalFinanceAPI.Infrastructure.SemanticKernel.Providers;

/// <summary>
/// Provedor de Kernel para Google Gemini (Generative AI)
/// </summary>
public class GeminiKernelProvider : IKernelProvider
{
	private readonly LLMProviderConfig _config;

	public string ProviderName => "gemini";

	public GeminiKernelProvider(LLMProviderConfig config)
	{
		_config = config;
	}

	public async Task<Kernel> CreateKernelAsync()
	{
		var builder = Kernel.CreateBuilder();

		//builder.AddGoogleGenerativeAIChatCompletion(
		//	modelId: _config.ModelId ?? "gemini-2.0-flash",
		//	apiKey: _config.ApiKey
		//);

		var kernel = builder.Build();

		return await Task.FromResult(kernel);
	}
}

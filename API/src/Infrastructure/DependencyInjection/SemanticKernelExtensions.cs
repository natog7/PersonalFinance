using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceAPI.Application.Features.Chat.Services;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Factory;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Interfaces;
using PersonalFinanceAPI.Infrastructure.SemanticKernel.Models;

namespace PersonalFinanceAPI.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods para registrar serviços do Semantic Kernel
/// </summary>
public static class SemanticKernelExtensions
{
	/// <summary>
	/// Registra os serviços do Semantic Kernel e Chat Service
	/// </summary>
	public static IServiceCollection AddSemanticKernel(this IServiceCollection services, IConfiguration configuration)
	{
		// Obter configuração do provedor LLM
		var llmConfig = new LLMProviderConfig
		{
			Provider = configuration["SemanticKernel:Provider"] ?? "openai",
			ApiKey = configuration["SemanticKernel:ApiKey"] ?? throw new InvalidOperationException("SemanticKernel:ApiKey não configurada"),
			BaseUrl = configuration["SemanticKernel:BaseUrl"],
			ModelId = configuration["SemanticKernel:ModelId"],
			MaxTokens = int.TryParse(configuration["SemanticKernel:MaxTokens"], out var maxTokens) ? maxTokens : 2000,
			Temperature = decimal.TryParse(configuration["SemanticKernel:Temperature"], out var temp) ? temp : 0.7m
		};

		// Registrar factory e provedor
		services.AddSingleton(llmConfig);
		services.AddSingleton<IKernelProvider>(sp => 
			KernelProviderFactory.CreateProvider(sp.GetRequiredService<LLMProviderConfig>()));

		// Registrar serviço de chat (singleton para reutilizar kernel)
		services.AddSingleton<IChatService, Services.ChatService>();

		return services;
	}
}

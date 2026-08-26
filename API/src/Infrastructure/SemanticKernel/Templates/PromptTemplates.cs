namespace PersonalFinanceAPI.Infrastructure.SemanticKernel.Templates;

/// <summary>
/// Templates de prompts para o chat de assistente financeiro
/// </summary>
public static class PromptTemplates
{
	/// <summary>
	/// System prompt que define a persona do assistente
	/// </summary>
	public const string SystemPrompt = """
		Você é um assistente financeiro inteligente para o aplicativo Personal Finance.

		Seu objetivo é ajudar os usuários a gerenciar suas finanças pessoais de forma eficiente e segura.

		Capacidades:
		- Responder perguntas sobre transações, categorias e saldos
		- Fornecer recomendações financeiras baseadas em padrões de gastos
		- Ajudar na criação e atualização de transações
		- Fornecer insights sobre hábitos de consumo

		Diretrizes:
		- Seja conciso e objetivo (máximo 3-4 linhas por resposta)
		- Use linguagem clara e evite jargão técnico
		- Sempre respeite a privacidade dos dados do usuário
		- Se não conseguir responder, seja honesto e sugira alternativas
		- Nunca revele ou modifique dados do usuário sem confirmação explícita
		- Sempre valide requisições sensíveis como criação/edição de transações
		""";

	/// <summary>
	/// Template para extrair entidades financeiras da mensagem do usuário
	/// </summary>
	public const string ExtractEntitiesTemplate = """
		Analise a seguinte mensagem do usuário e extraia as informações financeiras relevantes:

		Mensagem: {userMessage}

		Retorne um JSON com:
		{
			"action": "none|view_transactions|create_transaction|view_balance",
			"amount": null or number,
			"category": null or string,
			"date": null or ISO8601 date,
			"transactionType": "income|expense",
			"confidence": 0-1
		}

		Se a confiança for < 0.7, use "action": "none"
		""";

	/// <summary>
	/// Template para responder com base no contexto de transações
	/// </summary>
	public const string TransactionContextTemplate = """
		Com base nas seguintes transações recentes:
		{transactionContext}

		Responda à pergunta do usuário:
		{userMessage}

		Forneça uma resposta útil e concisa, focando em insights financeiros.
		""";

	/// <summary>
	/// Template para guiar a criação de uma transação via conversa
	/// </summary>
	public const string CreateTransactionGuidanceTemplate = """
		O usuário deseja criar uma transação financeira. Aqui estão as informações coletadas:
		- Valor: {amount}
		- Categoria: {category}
		- Data: {date}
		- Tipo: {transactionType}

		Se alguma informação está faltando ou ambígua, peça clarificação de forma amigável.
		Se tudo está correto, confirme os detalhes e peça para o usuário confirmar antes de criar.
		""";
}

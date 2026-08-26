namespace PersonalFinanceAPI.Application.Features.Chat;

public enum ChatRole
{
	User,
	Assistant,
	System
}

public record ChatMessageDto
{
	public required ChatRole Role { get; init; }
	public required string Content { get; init; }
	public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record ChatResponseDto
{
	public required string Message { get; init; }
	public string? Suggestion { get; init; }
	public bool RequiresConfirmation { get; init; }
	public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
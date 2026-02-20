namespace PersonalFinanceAPI.Application.Features.Auth;

public class TokenDto
{
	public string AccessToken { get; set; } = string.Empty;
	public string RefreshToken { get; set; } = string.Empty;
	public int ExpiresIn { get; set; } = 3600;
	public string TokenType { get; set; } = "Bearer";
}

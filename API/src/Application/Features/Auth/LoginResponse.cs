namespace PersonalFinanceAPI.Application.Features.Auth;

public class LoginResponse
{
	public Guid UserId { get; set; }
	public string Email { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public TokenDto Token { get; set; } = new();
}

using PersonalFinanceAPI.Application.Features.Auth.Commands;

namespace PersonalFinanceAPI.Application.Features.Auth;

public class RegisterCommand : IRequest<RegisterResponse?>
{
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
}

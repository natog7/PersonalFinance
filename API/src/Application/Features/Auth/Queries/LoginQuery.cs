using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Auth.Queries;

public record LoginQuery(string Email, string Password) : IRequest<LoginResponse?>;
public record LoginResponse(Guid UserId, string Email, string Nickname, TokenDto Token);
public record TokenDto(string AccessToken, string RefreshToken, int ExpiresIn = 3600, string TokenType = "Bearer");

public class LoginQueryHandler : CommandHandler<LoginQuery, LoginResponse?, IUserRepository>
{
	protected readonly ITokenService _tokenService;
	protected readonly IPasswordHasher _passwordHasher;

	public LoginQueryHandler(IUserRepository repository, ICurrentUserService userService, 
		ITokenService tokenService, IPasswordHasher passwordHasher)
		 : base(repository, userService)
	{
		_tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
		_passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
	}

	public override async Task<LoginResponse?> Handle(LoginQuery request, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
			return null;

		var user = await _repository.GetByEmailAsync(request.Email, ct);

		if (user is null || !user.IsActive || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
			return null;

		user.UpdateLastLogin();
		await _repository.UpdateAsync(user, ct);

		var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role);
		var refreshToken = _tokenService.GenerateRefreshToken();

		return new LoginResponse(user.Id, user.Email, user.Nickname, new TokenDto(accessToken, refreshToken));
	}
}

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
	public LoginQueryValidator()
	{
		RuleFor(x => x.Email).IsEmail();
		RuleFor(x => x.Password).IsPassword();
	}
}
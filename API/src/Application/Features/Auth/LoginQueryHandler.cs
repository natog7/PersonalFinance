using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Application.Repositories;

namespace PersonalFinanceAPI.Application.Features.Auth.Queries;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginQueryHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<LoginResponse?> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return null;

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !user.IsActive || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return null;

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.Nickname,
            Token = new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 3600
            }
        };
    }
}
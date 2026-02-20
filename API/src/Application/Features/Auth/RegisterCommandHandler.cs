using MediatR;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Domain.Entities;
using FluentValidation;

namespace PersonalFinanceAPI.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<RegisterResponse?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);
        if (emailExists)
            return null; // Email already exists

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FullName);

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.Nickname
        };
    }
}

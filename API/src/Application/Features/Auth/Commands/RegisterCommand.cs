using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password, string Nickname) : IRequest<UserDto?>;

public class RegisterCommandHandler : CommandHandler<RegisterCommand, UserDto?, IUserRepository>
{
	private readonly IPasswordHasher _passwordHasher;

	public RegisterCommandHandler(IUserRepository repository, ICurrentUserService userService, IPasswordHasher passwordHasher)
		 : base(repository, userService)
	{
		_passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
	}

	public override async Task<UserDto?> Handle(RegisterCommand request, CancellationToken ct)
	{
		var emailExists = await _repository.EmailExistsAsync(request.Email, ct);
		if (emailExists)
			return null; // Email already exists

		var passwordHash = _passwordHasher.HashPassword(request.Password);
		var user = User.Create(request.Email, passwordHash, request.Nickname);

		await _repository.AddAsync(user, ct);

		return new UserDto
		{
			Id = user.Id,
			Email = user.Email,
			Nickname = user.Nickname,
			Currency = user.Currency,
			DarkTheme = user.DarkTheme
		};
	}
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
	public RegisterCommandValidator()
	{
		RuleFor(x => x.Email).IsEmail();
		RuleFor(x => x.Password).IsPassword();
		RuleFor(x => x.Nickname).NotEmptyMinMaxLength(3, 128);
	}
}
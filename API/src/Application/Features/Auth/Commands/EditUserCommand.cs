using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Auth.Commands;

public record EditUserCommand(string Email, string Password, string Nickname, string? Currency, bool? DarkTheme) : IRequest<UserDto?>;

public class EditUserCommandHandler : CommandHandler<EditUserCommand, UserDto?, IUserRepository>
{
	private readonly IPasswordHasher _passwordHasher;

	public EditUserCommandHandler(IUserRepository repository, ICurrentUserService userService, IPasswordHasher passwordHasher)
		 : base(repository, userService)
	{
		_passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
	}

	public override async Task<UserDto?> Handle(EditUserCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		if (!_userService.UserId.HasValue)
		{
			throw new Exception("Authenticated user ID is missing.");
		}

		var user = await _repository.GetByIdAsync(_userService.UserId.Value, ct);
		if (user == null)
			return null;

		var passwordHash = _passwordHasher.HashPassword(request.Password);

		// Check if email is being changed and if it already exists
		if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
		{
			var emailExists = await _repository.EmailExistsAsync(request.Email, ct);
			if (emailExists)
				throw new Exception("Email already registered by another user.");
		}

		user.Update(request.Email, passwordHash, request.Nickname, request.Currency, request.DarkTheme);

		await _repository.UpdateAsync(user, ct);

		return new UserDto()
		{
			Id = user.Id,
			Email = user.Email,
			Nickname = user.Nickname,
			Currency = user.Currency,
			DarkTheme = user.DarkTheme
		};
	}
}

public class EditUserCommandValidator : AbstractValidator<EditUserCommand>
{
	public EditUserCommandValidator()
	{
		RuleFor(x => x.Email).IsEmail();
		RuleFor(x => x.Nickname).NotEmptyMinMaxLength(3, 128);
		RuleFor(x => x.Currency).NotEmptyLength(3);
	}
}

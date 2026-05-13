using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Auth.Commands;

public record EditUserCommand(string? Email, string? OldPassword, string? NewPassword, string? Nickname, string? Currency, bool? DarkTheme) : IRequest<UserDto?>;

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

		string? oldPasswordHash = null;
		string? newPasswordHash = null;
		if (!string.IsNullOrWhiteSpace(request.OldPassword) && !string.IsNullOrWhiteSpace(request.NewPassword))
		{
			oldPasswordHash = request.OldPassword != null ? _passwordHasher.HashPassword(request.OldPassword) : null;
			newPasswordHash = request.NewPassword != null ? _passwordHasher.HashPassword(request.NewPassword) : null;
		}

		// Check if email is being changed and if it already exists
		if (request.Email != null && !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
		{
			var emailExists = await _repository.EmailExistsAsync(request.Email, ct);
			if (emailExists)
				throw new Exception("Email already registered by another user.");
		}

		user.Update(request.Email, newPasswordHash, request.Nickname, request.Currency, request.DarkTheme);

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
       RuleFor(x => x.Email)
			.IsEmailNull()
			.When(x => !string.IsNullOrWhiteSpace(x.Email));

		RuleFor(x => x.Nickname)
			.NotEmptyMinMaxLengthNull(3, 128)
            .When(x => !string.IsNullOrWhiteSpace(x.Nickname));

       RuleFor(x => x.Currency)
			.NotEmptyLengthNull(3)
			.When(x => !string.IsNullOrWhiteSpace(x.Currency));

		RuleFor(x => x.OldPassword)
			 .IsPasswordNull()
			 .When(x => !string.IsNullOrWhiteSpace(x.OldPassword));

		RuleFor(x => x.NewPassword)
			 .IsPasswordNull()
			 .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));

		RuleFor(x => x)
           .Must(x => string.IsNullOrEmpty(x.OldPassword) || string.IsNullOrEmpty(x.NewPassword) || x.OldPassword != x.NewPassword)
           .WithMessage("Old password and new password must be different.");
	}
}

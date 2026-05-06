using PersonalFinanceAPI.Application.Extensions;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Domain.Services;

namespace PersonalFinanceAPI.Application.Features.Auth.Commands;

public record EditUserCommand(string Email, string Nickname, string? Currency, bool? DarkTheme) : IRequest<EditUserResponse?>;
public record EditUserResponse(Guid UserId, string Email, string Nickname, string Currency, bool DarkTheme);

public class EditUserCommandHandler : CommandHandler<EditUserCommand, EditUserResponse?, IUserRepository>
{
	public EditUserCommandHandler(IUserRepository repository, ICurrentUserService userService)
		 : base(repository, userService)
	{
	}

	public override async Task<EditUserResponse?> Handle(EditUserCommand request, CancellationToken ct)
	{
		CheckAuthenticated();

		if (!_userService.UserId.HasValue)
		{
			throw new Exception("Authenticated user ID is missing.");
		}

		var user = await _repository.GetByIdAsync(_userService.UserId.Value, ct);
		if (user == null)
			return null;

		// Check if email is being changed and if it already exists
		if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
		{
			var emailExists = await _repository.EmailExistsAsync(request.Email, ct);
			if (emailExists)
				throw new Exception("Email already registered by another user.");
		}

		user.Update(request.Email, request.Nickname, request.Currency, request.DarkTheme);

		await _repository.UpdateAsync(user, ct);

		return new EditUserResponse(user.Id, user.Email, user.Nickname, user.Currency, user.DarkTheme);
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

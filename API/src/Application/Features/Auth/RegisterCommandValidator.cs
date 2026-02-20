namespace PersonalFinanceAPI.Application.Features.Auth;


public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
	public RegisterCommandValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email must be valid.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(8).WithMessage("Password must be at least 8 characters.")
			.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			.Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
			.Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character.");

		RuleFor(x => x.FullName)
			.NotEmpty().WithMessage("Full name is required.")
			.MaximumLength(256).WithMessage("Full name cannot exceed 256 characters.");
	}
}

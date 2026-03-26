namespace PersonalFinanceAPI.Application.Extensions;

public static class ValidationExtensions
{
	public static IRuleBuilderOptions<T, TProperty> NotEmptyOrNull<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
	{
		return ruleBuilder
			.NotEmpty().WithMessage("{PropertyName} must not be empty.")
			.NotNull().WithMessage("{PropertyName} must not be null.");
	}

	public static IRuleBuilderOptions<T, string?> NotEmptyMaxLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int length)
	{
		return ruleBuilder
			.NotEmptyOrNull()
			.MaximumLength(length).WithMessage(string.Format("{PropertyName} cannot exceed {0} characters.", length));
	}

	public static IRuleBuilderOptions<T, string?> NotEmptyLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int length)
	{
		return ruleBuilder
			.NotEmptyOrNull()
			.Length(length).WithMessage(string.Format("{PropertyName} must be {0} characters.", length));
	}

	public static IRuleBuilderOptions<T, string?> IsHexColor<T>(this IRuleBuilder<T, string?> ruleBuilder)
	{
		return ruleBuilder
			.Matches(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$").WithMessage("{PropertyName} must be a valid hexadecimal color (e.g. #FFFFFF or #FFF).");
		//.When(x => !string.IsNullOrWhiteSpace(x));
	}

	public static IRuleBuilderOptions<T, string?> IsEmail<T>(this IRuleBuilder<T, string?> ruleBuilder)
	{
		return ruleBuilder
			.NotEmptyOrNull()
			.EmailAddress().WithMessage("{PropertyName} must be in a valid format (e.g. user@email.com).");
	}

	public static IRuleBuilderOptions<T, string?> IsPhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
	{
		return ruleBuilder
			.Matches(@"^\(?\d{2}\)?[\s-]?\d{4,5}-?\d{4}$").WithMessage("{PropertyName} must be in a valid format (e.g. (11) 99999-9999).");
	}

	public static IRuleBuilderOptions<T, string?> IsPassword<T>(this IRuleBuilder<T, string?> ruleBuilder)
	{
		return ruleBuilder
			.NotEmptyOrNull()
			.MinimumLength(8).WithMessage("{PropertyName} must be at least 8 characters.")
			.Matches(@"[A-Z]").WithMessage("{PropertyName} must contain at least one uppercase letter.")
			.Matches(@"[0-9]").WithMessage("{PropertyName} must contain at least one digit.")
			.Matches(@"[!@#$%^&*]").WithMessage("{PropertyName} must contain at least one special character.");
	}

	public static IRuleBuilderOptions<T, int> IsInclusiveBetween<T>(this IRuleBuilder<T, int> ruleBuilder, int min, int max)
	{
		return ruleBuilder
			.NotEmptyOrNull()
			.InclusiveBetween(min, max).WithMessage(string.Format("{PropertyName} must be between {0} and {1}, both inclusive.", min, max));
	}

	public static IRuleBuilderOptions<T, int> IsExclusiveBetween<T>(this IRuleBuilder<T, int> ruleBuilder, int min, int max)
	{
		return ruleBuilder
			.NotEmptyOrNull()
			.ExclusiveBetween(min, max).WithMessage(string.Format("{PropertyName} must be between {0} and {1}, both inclusive.", min + 1, max - 1));
	}
}

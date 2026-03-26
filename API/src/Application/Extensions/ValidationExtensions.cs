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
			.NotEmpty().WithMessage("{PropertyName} must not be empty.")
			.MaximumLength(length).WithMessage(string.Format("{PropertyName} cannot exceed {0} characters.", length));
	}

	public static IRuleBuilderOptions<T, string?> NotEmptyLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int length)
	{
		return ruleBuilder
			.NotEmpty().WithMessage("{PropertyName} must not be empty.")
			.Length(length).WithMessage(string.Format("{PropertyName} must be {0} characters.", length));
	}

	public static IRuleBuilderOptions<T, string?> IsHexColor<T>(this IRuleBuilder<T, string?> ruleBuilder)
	{
		return ruleBuilder
			.Matches(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$").WithMessage("{PropertyName} must be a valid hexadecimal color (e.g. #FFFFFF or #FFF).");
			//.When(x => !string.IsNullOrWhiteSpace(x));
	}

	public static IRuleBuilderOptions<T, string?> IsPhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
	{
		return ruleBuilder
			.Matches(@"^\(?\d{2}\)?[\s-]?\d{4,5}-?\d{4}$")
			.WithMessage("{PropertyName} must be in a valid formad (e.g. (11) 99999-9999).");
	}
}

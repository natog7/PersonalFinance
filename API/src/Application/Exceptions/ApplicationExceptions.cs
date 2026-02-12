namespace PersonalFinanceAPI.Application.Exceptions;

/// <summary>
/// Base exception for application-level errors.
/// </summary>
public class ApplicationException : Exception
{
    public string ErrorCode { get; }

    public ApplicationException(string message, string errorCode = "APP_ERROR") : base(message)
    {
        ErrorCode = errorCode;
    }

    public ApplicationException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class ResourceNotFoundException : ApplicationException
{
    public ResourceNotFoundException(string resourceName, Guid id)
        : base($"Resource '{resourceName}' with ID '{id}' was not found.", "NOT_FOUND") { }

    public ResourceNotFoundException(string message)
        : base(message, "NOT_FOUND") { }
}

/// <summary>
/// Exception thrown when business rule validation fails.
/// </summary>
public class BusinessRuleViolationException : ApplicationException
{
    public BusinessRuleViolationException(string message)
        : base(message, "BUSINESS_RULE_VIOLATION") { }
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : ApplicationException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Validation failed.", "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string message)
        : base(message, "VALIDATION_ERROR")
    {
        Errors = new();
    }
}

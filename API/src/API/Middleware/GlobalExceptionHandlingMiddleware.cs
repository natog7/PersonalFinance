namespace PersonalFinanceAPI.API.Middleware;

/// <summary>
/// Global exception handling middleware for consistent error responses.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        return exception switch
        {
            ArgumentNullException => RespondWith(context, StatusCodes.Status400BadRequest, response),
            ArgumentException => RespondWith(context, StatusCodes.Status400BadRequest, response),
            InvalidOperationException => RespondWith(context, StatusCodes.Status400BadRequest, response),
            _ => RespondWith(context, StatusCodes.Status500InternalServerError, response)
        };
    }

    private static Task RespondWith(HttpContext context, int statusCode, ErrorResponse response)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

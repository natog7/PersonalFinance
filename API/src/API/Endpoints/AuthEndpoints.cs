using MediatR;
using Microsoft.AspNetCore.Authorization;
using PersonalFinanceAPI.Application.Features.Auth.Commands;
using PersonalFinanceAPI.Application.Features.Auth.Queries;

namespace PersonalFinanceAPI.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithName("Authentication");

        group.MapPost("/register", Register)
            .WithName("Register")
			.RequireAuthorization()
			.Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);

        group.MapPost("/login", Login)
            .WithName("Login")
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh-token", RefreshToken)
            .WithName("Refresh Token")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Register(
        RegisterCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(command, cancellationToken);
            
            if (result is null)
                return Results.Conflict(new { error = "Email already registered" });

            return Results.Created($"/api/auth/user/{result.UserId}", result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            var errors = ex.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return Results.BadRequest(new { errors });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new LoginQuery { Email = request.Email, Password = request.Password };
            var result = await mediator.Send(query, cancellationToken);

            if (result is null)
                return Results.Unauthorized();

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> Logout(
        HttpContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            // Token blacklist logic can be implemented here if needed
            return Results.Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RefreshToken(
        RefreshTokenRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            // Implement refresh token rotation logic
            return Results.Ok(new { message = "Token refreshed successfully" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

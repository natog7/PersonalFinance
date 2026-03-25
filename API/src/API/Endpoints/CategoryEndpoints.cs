using MediatR;
using PersonalFinanceAPI.Application.Features.Categories;
using System.Security.Claims;

namespace PersonalFinanceAPI.API.Endpoints;

public static class CategoryEndpoints
{
	public static void MapCategoryEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/api/categories")
			.WithName("Categories")
			.RequireAuthorization();

		group.MapPost("/", CreateCategory)
			.WithName("Create Category")
			.RequireAuthorization()
			.Produces(StatusCodes.Status201Created)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized);

		group.MapGet("/{id}", GetCategory)
			.WithName("Get Category")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound)
			.Produces(StatusCodes.Status401Unauthorized);

		group.MapPost("/filter/", GetCategories)
			.WithName("Get Categories")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized);

		group.MapPut("/{id}", UpdateCategory)
			.WithName("Update Category")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized);

		group.MapDelete("/{id}", DeleteCategory)
			.WithName("Delete Category")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized);
	}

	private static async Task<IResult> CreateCategory(
		CreateCategoryCommand command,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			var result = await mediator.Send(command, cancellationToken);
			return Results.Created($"/api/categories/{result.Id}", result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}

	private static async Task<IResult> GetCategory(
		Guid id,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
			var query = new GetCategoryQuery { Id = id };

			var result = await mediator.Send(query, cancellationToken);
			return result is null ? Results.NotFound() : Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}

	private static async Task<IResult> GetCategories(
		GetCategoriesQuery query,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			var result = await mediator.Send(query, cancellationToken);
			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}

	private static async Task<IResult> UpdateCategory(
	Guid id,
	UpdateCategoryCommand command,
	ClaimsPrincipal user,
	IMediator mediator,
	CancellationToken cancellationToken)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			if (id != command.Id)
			{
				return Results.BadRequest(new { error = "The URL ID doesn't match the request body ID." });
			}

			var result = await mediator.Send(command, cancellationToken);

			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}

	private static async Task<IResult> DeleteCategory(
		Guid id,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			await mediator.Send(new DeleteCategoryCommand(id), cancellationToken);

			return Results.Ok(new { message = "Successfully deleted." });
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}
}
using PersonalFinanceAPI.Application.Exceptions;
using PersonalFinanceAPI.Application.Features.Categories;
using PersonalFinanceAPI.Application.Features.Categories.Commands;
using PersonalFinanceAPI.Application.Features.Categories.Queries;

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
			.Produces<IdDto<Guid>>(StatusCodes.Status201Created)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
			.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

		group.MapGet("/{id}", GetCategory)
			.WithName("Get Category")
			.RequireAuthorization()
			.Produces<CategoryDto>(StatusCodes.Status200OK)
			.Produces<ErrorResponse>(StatusCodes.Status404NotFound)
			.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

		group.MapPost("/filter/", GetCategories)
			.WithName("List Categories")
			.RequireAuthorization()
			.Produces<ListResult<CategoryDto>>(StatusCodes.Status200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
			.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

		group.MapPut("/{id}", UpdateCategory)
			.WithName("Update Category")
			.RequireAuthorization()
			.Produces<CategoryDto>(StatusCodes.Status200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
			.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

		group.MapDelete("/{id}", DeleteCategory)
			.WithName("Delete Category")
			.RequireAuthorization()
			.Produces<MessageResponse>(StatusCodes.Status200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
			.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
	}

	private static async Task<IResult> CreateCategory(
		CreateCategoryCommand command,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken ct)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			var result = await mediator.Send(command, ct);
			return Results.Created($"/api/categories/{result.Id}", result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new ErrorResponse(ex.Message));
		}
	}

	private static async Task<IResult> GetCategory(
		Guid id,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken ct)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
			var query = new GetByIdQuery<CategoryDto?>(id);

			var result = await mediator.Send(query, ct);
			return result is null ? Results.NotFound() : Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new ErrorResponse(ex.Message));
		}
	}

	private static async Task<IResult> GetCategories(
		GetCategoriesQuery query,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken ct)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			var result = await mediator.Send(query, ct);
			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new ErrorResponse(ex.Message));
		}
	}

	private static async Task<IResult> UpdateCategory(
	Guid id,
	UpdateCategoryCommand command,
	ClaimsPrincipal user,
	IMediator mediator,
	CancellationToken ct)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			if (id != command.Id)
			{
				return Results.BadRequest(new { error = "The URL ID doesn't match the request body ID." });
			}

			var result = await mediator.Send(command, ct);

			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new ErrorResponse(ex.Message));
		}
	}

	private static async Task<IResult> DeleteCategory(
		Guid id,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken ct)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			await mediator.Send(new DeleteCommand(id), ct);

			return Results.Ok(new MessageResponse("Successfully deleted."));
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new ErrorResponse(ex.Message));
		}
	}
}
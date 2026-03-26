using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Features.Transactions.Commands;
using PersonalFinanceAPI.Application.Features.Transactions.Queries;

namespace PersonalFinanceAPI.API.Endpoints;

/// <summary>
/// Minimal API endpoints for transaction operations.
/// </summary>
public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/transactions")
            .WithName("Transactions")
            .RequireAuthorization();

        group.MapPost("/", CreateTransaction)
            .WithName("Create Transaction")
			.RequireAuthorization()
			.Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id}", GetTransaction)
            .WithName("Get Transaction")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/filter/", GetTransactions)
            .WithName("List Transactions")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

		group.MapPut("/{id}", UpdateTransaction)
			.WithName("Update Transaction")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized);

		group.MapDelete("/{id}", DeleteTransaction)
			.WithName("Delete Transaction")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> CreateTransaction(
        CreateTransactionCommand command,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken ct)
    {
        try
        {
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

			var result = await mediator.Send(command, ct);
            return Results.Created($"/api/transactions/{result.Id}", result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetTransaction(
        Guid id,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken ct)
    {
        try
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var query = new GetByIdQuery<TransactionDto?>(id);
            
            var result = await mediator.Send(query, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetTransactions(
        GetTransactionsQuery query,
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
            return Results.BadRequest(new { error = ex.Message });
        }
	}

	private static async Task<IResult> UpdateTransaction(
	Guid id,
	UpdateTransactionCommand command,
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
			return Results.BadRequest(new { error = ex.Message });
		}
	}

	private static async Task<IResult> DeleteTransaction(
		Guid id,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken ct)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            await mediator.Send(new DeleteCommand(id), ct);

            return Results.Ok(new { message = "Successfully deleted." });
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}
}
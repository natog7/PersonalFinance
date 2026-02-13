using MediatR;
using PersonalFinanceAPI.Application.Features.Transactions;
using PersonalFinanceAPI.Application.Queries;

namespace PersonalFinanceAPI.API.Endpoints;

/// <summary>
/// Minimal API endpoints for transaction operations.
/// </summary>
public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/transactions")
            .WithName("Transactions");

        group.MapPost("/", CreateTransaction)
            .WithName("Create Transaction")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

		group.MapGet("/{id}", GetTransaction)
			.WithName("Get Transaction")
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapPost("/filter/", GetTransactions)
            .WithName("Get Transactions")
            .Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest);

		group.MapGet("/balance-projection/{categoryId}", GetBalanceProjection)
            .WithName("Get Balance Projection")
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> CreateTransaction(
        CreateTransactionCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(command, cancellationToken);
			return Results.Created($"/api/transactions/{result.Id}", result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}

	private static async Task<IResult> GetTransaction(
		Guid id,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var query = new GetTransactionQuery { Id = id };
			var result = await mediator.Send(query, cancellationToken);
			return result is null ? Results.NotFound() : Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}

	private static async Task<IResult> GetTransactions(
		[AsParameters] GetTransactionsQuery query,
		IMediator mediator,
		CancellationToken cancellationToken)
    {
        try
        {
			var result = await mediator.Send(query, cancellationToken);
			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new { error = ex.Message });
		}
	}

	private static async Task<IResult> GetBalanceProjection(
        Guid categoryId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetBalanceProjectionQuery { CategoryId = categoryId };
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}
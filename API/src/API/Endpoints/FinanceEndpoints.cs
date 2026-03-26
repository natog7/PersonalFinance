using PersonalFinanceAPI.Application.Features.Finance;

namespace PersonalFinanceAPI.API.Endpoints;

public static class FinanceEndpoints
{
	public static void MapFinanceEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/api/finance")
			.WithName("Finance")
			.RequireAuthorization();

		group.MapPost("/balance-projection/", GetBalanceProjection)
			.WithName("Get Balance Projection")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized);
	}

	private static async Task<IResult> GetBalanceProjection(
		GetBalanceProjectionQuery query,
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
}

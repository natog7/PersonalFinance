using PersonalFinanceAPI.Application.Exceptions;
using PersonalFinanceAPI.Application.Features.Categories;
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
			.Produces<ListResult<MonthlyProjection>>(StatusCodes.Status200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
			.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

		group.MapPost("/transactions-by-category/", GetTransactionsByCategory)
			.WithName("Get Transactions By Category")
			.RequireAuthorization()
			.Produces<ListResult<CategoryTransactionSumDto>>(StatusCodes.Status200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
			.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

		group.MapPost("/balance-by-month/", GetBalanceByMonth)
			.WithName("Get Balance By Month")
			.RequireAuthorization()
			.Produces<ListResult<BalanceByMonthDto>>(StatusCodes.Status200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
			.Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
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
			return Results.BadRequest(new ErrorResponse(ex.Message));
		}
	}
	
	private static async Task<IResult> GetTransactionsByCategory(
		GetTransactionsByCategoryQuery query,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken ct)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
			var result = await mediator.Send(query, ct);
			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new ErrorResponse(ex.Message));
		}
	}

	private static async Task<IResult> GetBalanceByMonth(
		GetBalanceByMonthQuery query,
		ClaimsPrincipal user,
		IMediator mediator,
		CancellationToken ct)
	{
		try
		{
			var userId = Guid.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
			var result = await mediator.Send(query, ct);
			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest(new ErrorResponse(ex.Message));
		}
	}
}

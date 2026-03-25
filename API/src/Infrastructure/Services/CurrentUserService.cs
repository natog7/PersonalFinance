using Microsoft.AspNetCore.Http;
using PersonalFinanceAPI.Domain.Services;
using System.Security.Claims;

namespace PersonalFinanceAPI.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
	public Guid? UserId
	{
		get
		{
			var id = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return Guid.TryParse(id, out var userId) ? userId : null;
		}
	}

	public bool isAuthenticated => UserId.HasValue;
}

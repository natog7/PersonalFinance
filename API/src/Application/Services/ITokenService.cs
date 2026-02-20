using PersonalFinanceAPI.Domain.Enums;
using System.Security.Claims;

namespace PersonalFinanceAPI.Application.Services;


public interface ITokenService
{
	string GenerateAccessToken(Guid userId, string email, UserRole role);
	string GenerateRefreshToken();
	ClaimsPrincipal? ValidateToken(string token);
}
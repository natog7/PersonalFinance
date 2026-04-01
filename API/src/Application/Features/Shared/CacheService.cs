using StackExchange.Redis;

namespace PersonalFinanceAPI.Application.Features.Shared;

public class CacheService
{
	protected readonly IConnectionMultiplexer _redis;
	protected readonly TimeSpan _expiry = TimeSpan.FromHours(1);

	public CacheService(IConnectionMultiplexer redis) => _redis = redis;
}

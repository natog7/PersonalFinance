using PersonalFinanceAPI.Application.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace PersonalFinanceAPI.Application.Features.Finance.Services;

public class BalanceProjectionCacheService : IBalanceProjectionCacheService
{
	private readonly IConnectionMultiplexer _redis;
	private readonly TimeSpan _expiry = TimeSpan.FromHours(1);

	public BalanceProjectionCacheService(IConnectionMultiplexer redis) => _redis = redis;

	public async Task<ListResult<MonthlyProjection>?> GetAsync(string key, CancellationToken ct = default)
	{
		var db = _redis.GetDatabase();
		var value = await db.StringGetAsync(key);
		return value.HasValue ? JsonSerializer.Deserialize<ListResult<MonthlyProjection>>(utf8Json: value!) : null;
	}

	public async Task SetAsync(string key, ListResult<MonthlyProjection> value, CancellationToken ct = default)
	{
		var db = _redis.GetDatabase();
		await db.StringSetAsync(key, JsonSerializer.Serialize(value), _expiry);
	}
}
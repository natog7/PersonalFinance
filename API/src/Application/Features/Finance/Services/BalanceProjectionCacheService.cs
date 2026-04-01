using PersonalFinanceAPI.Application.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace PersonalFinanceAPI.Application.Features.Finance.Services;

public class BalanceProjectionCacheService : CacheService, ICacheService<ListResult<MonthlyProjection>>
{
	public BalanceProjectionCacheService(IConnectionMultiplexer redis) : base(redis) { }

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
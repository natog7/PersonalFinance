using MongoDB.Driver;
using PersonalFinanceAPI.Application.Features.Finance;
using PersonalFinanceAPI.Application.Features.Shared;
using PersonalFinanceAPI.Application.Repositories;
using PersonalFinanceAPI.Infrastructure.Persistence.Documents;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

public class BalanceProjectionRepository : IGetSetRepository<ListResult<MonthlyProjection>>
{
	protected readonly IMongoCollection<MongoDocument<ListResult<MonthlyProjection>>> _collection;

	public BalanceProjectionRepository(IMongoDatabase database)
	{
		_collection = database.GetCollection<MongoDocument<ListResult<MonthlyProjection>>>("balance_projections");
	}

	public async Task<ListResult<MonthlyProjection>?> GetAsync(string key, CancellationToken ct = default)
	{
		var doc = await _collection.Find(x => x.Key == key).FirstOrDefaultAsync(ct);
		return doc?.Content;
	}

	public async Task SaveAsync(string key, ListResult<MonthlyProjection> value, CancellationToken ct = default)
	{
		var doc = new MongoDocument<ListResult<MonthlyProjection>>
		{
			Key = key,
			Content = value,
			CreatedAt = DateTime.UtcNow
		};
		await _collection.ReplaceOneAsync(
			x => x.Key == key,
			doc,
			new ReplaceOptions { IsUpsert = true },
			ct);
	}
}

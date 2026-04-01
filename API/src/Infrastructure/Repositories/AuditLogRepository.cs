using MongoDB.Driver;
using PersonalFinanceAPI.Infrastructure.Persistence.Documents;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
	protected readonly IMongoCollection<AuditLog> _collection;

	public AuditLogRepository(IMongoDatabase database)
	{
		_collection = database.GetCollection<AuditLog>("audit_logs");
		EnsureIndexes();
	}

	protected void EnsureIndexes()
	{
		var indexes = new[]
		{
			new CreateIndexModel<AuditLog>(
				Builders<AuditLog>.IndexKeys
					.Ascending(x => x.EntityName)
					.Ascending(x => x.EntityId)),
			new CreateIndexModel<AuditLog>(
				Builders<AuditLog>.IndexKeys
					.Ascending(x => x.UserId)),
			new CreateIndexModel<AuditLog>(
				Builders<AuditLog>.IndexKeys
					.Descending(x => x.OccurredAt),
				new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(90) })
        };
		_collection.Indexes.CreateMany(indexes);
	}

	public Task InsertAsync(AuditLog log, CancellationToken ct = default)
		=> _collection.InsertOneAsync(log, cancellationToken: ct);

	public async Task<List<AuditLog>> GetByEntityAsync(string entityName, string entityId, CancellationToken ct = default)
		=> await _collection
			.Find(x => x.EntityName == entityName && x.EntityId == entityId)
			.SortByDescending(x => x.OccurredAt)
			.ToListAsync(ct);

	public async Task<List<AuditLog>> GetByUserAsync(Guid userId, int limit = 50, CancellationToken ct = default)
		=> await _collection
			.Find(x => x.UserId == userId)
			.SortByDescending(x => x.OccurredAt)
			.Limit(limit)
			.ToListAsync(ct);
}
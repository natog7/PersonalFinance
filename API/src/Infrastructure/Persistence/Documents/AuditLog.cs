using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;

namespace PersonalFinanceAPI.Infrastructure.Persistence.Documents;

public enum AuditAction { Created, Updated, Deleted, Read }

public class AuditLog
{
	[BsonId]
	public Guid Id { get; protected set; } = Guid.NewGuid();
	public string EntityName { get; protected set; } = default!;
	public string EntityId { get; protected set; } = default!;
	public AuditAction Action { get; protected set; }
	public Guid? UserId { get; protected set; }
	public DateTime OccurredAt { get; protected set; } = DateTime.UtcNow;
	public BsonDocument? Before { get; protected set; }
	public BsonDocument? After { get; protected set; }
	public string? Metadata { get; protected set; }

	protected AuditLog() { }

	public static AuditLog Create(
		string entityName,
		string entityId,
		AuditAction action,
		Guid? userId = null,
		object? before = null,
		object? after = null,
		string? metadata = null) => new()
		{
			EntityName = entityName,
			EntityId = entityId,
			Action = action,
			UserId = userId,
			Before = before is null ? null : BsonDocument.Parse(JsonSerializer.Serialize(before)),
			After = after is null ? null : BsonDocument.Parse(JsonSerializer.Serialize(after)),
			Metadata = metadata
		};
}

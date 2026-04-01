using MongoDB.Bson.Serialization.Attributes;

namespace PersonalFinanceAPI.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
public class MongoDocument<T>
{
	[BsonId]
	public string Key { get; set; } = default!;
	public T Content { get; set; } = default!;
	public DateTime CreatedAt { get; set; }
}

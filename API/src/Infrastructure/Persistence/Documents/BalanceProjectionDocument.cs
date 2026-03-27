using MongoDB.Bson.Serialization.Attributes;
using PersonalFinanceAPI.Application.Features.Finance;
using PersonalFinanceAPI.Application.Features.Shared;

namespace PersonalFinanceAPI.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
public class BalanceProjectionDocument
{
	[BsonId]
	public string Key { get; set; } = default!;
	public ListResult<MonthlyProjection> Projection { get; set; } = default!;
	public DateTime CreatedAt { get; set; }
}

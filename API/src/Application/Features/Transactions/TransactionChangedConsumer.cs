using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using PersonalFinanceAPI.Domain.Events;

namespace PersonalFinanceAPI.Application.Features.Transactions;

public class TransactionChangedConsumer(IMongoDatabase mongoDb, IDistributedCache cache)
	: IConsumer<TransactionChangedEvent>
{
	public async Task Consume(ConsumeContext<TransactionChangedEvent> context)
	{
		var msg = context.Message;
		var collection = mongoDb.GetCollection<MonthlySummary>("MonthlySummaries");

		//// MongoDB update
		//var filter = Builders<MonthlySummary>.Filter.Where(x =>
		//	x.UserId == msg.UserId && x.Month == msg.Month && x.Year == msg.Year && x.Currency == msg.Currency);

		//var update = msg.IsIncome
		//	? Builders<MonthlySummary>.Update.Inc(x => x.TotalIncome, msg.Amount)
		//	: Builders<MonthlySummary>.Update.Inc(x => x.TotalExpense, msg.Amount);

		//await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });

		// Invalida o Cache do Redis para este usuário pois os dados mudaram
		await cache.RemoveAsync($"projection:{msg.UserId}");
	}
}

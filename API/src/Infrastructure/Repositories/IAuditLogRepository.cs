using PersonalFinanceAPI.Infrastructure.Persistence.Documents;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

public interface IAuditLogRepository
{
	Task InsertAsync(AuditLog log, CancellationToken ct = default);
	Task<List<AuditLog>> GetByEntityAsync(string entityName, string entityId, CancellationToken ct = default);
	Task<List<AuditLog>> GetByUserAsync(Guid userId, int limit = 50, CancellationToken ct = default);
}
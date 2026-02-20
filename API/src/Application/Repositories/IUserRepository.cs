namespace PersonalFinanceAPI.Application.Repositories;

public interface IUserRepository : IRepository<Domain.Entities.User, Guid>
{
    Task<Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

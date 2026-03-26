using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Domain.Entities;
using PersonalFinanceAPI.Infrastructure.Persistence;
using PersonalFinanceAPI.Application.Repositories;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

public class UserRepository : BaseRepository, IUserRepository
{
	public UserRepository(ApplicationDbContext dbContext) : base(dbContext) { }

	public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Email == email.ToLower(), ct);
    }

    public async Task AddAsync(User entity, CancellationToken ct = default)
    {
        _dbContext.Users.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User entity, CancellationToken ct = default)
    {
        _dbContext.Users.Update(entity);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await GetByIdAsync(id, ct);
        if (user is not null)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public IQueryable<User> GetQueryable()
    {
        return _dbContext.Users.AsQueryable();
    }
}

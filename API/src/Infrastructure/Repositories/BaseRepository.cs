using PersonalFinanceAPI.Infrastructure.Persistence;

namespace PersonalFinanceAPI.Infrastructure.Repositories;

public class BaseRepository
{
	protected readonly ApplicationDbContext _dbContext;

	public BaseRepository(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}
}

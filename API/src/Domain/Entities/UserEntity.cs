using PersonalFinanceAPI.Domain.Entities.Interfaces;

namespace PersonalFinanceAPI.Domain.Entities;

public abstract class UserEntity<TId> : Entity<TId>, IEntityFields<TId> where TId : struct, IEquatable<TId>
{
	public TId? UserId { get; protected set; }
}

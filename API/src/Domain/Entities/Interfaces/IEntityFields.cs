namespace PersonalFinanceAPI.Domain.Entities.Interfaces;

public interface IEntityFields<TId> where TId : struct, IEquatable<TId>
{
	public TId Id { get; }
}

namespace PersonalFinanceAPI.Domain.Entities;

/// <summary>
/// Base entity class for all domain entities.
/// </summary>
public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : struct, IEquatable<TId>
{
    public TId Id { get; protected set; }

    protected Entity(TId id) => Id = id;
    protected Entity() { }

    public override bool Equals(object? obj) => obj is Entity<TId> entity && Equals(entity);

    public bool Equals(Entity<TId>? other) => other is not null && Id.Equals(other.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
        !Equals(left, right);
}

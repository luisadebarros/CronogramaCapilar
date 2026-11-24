namespace CronogramaCapilar.Domain.Common;

/// <summary>
/// Base de todas as entidades. Uma entidade é identificada pelo <see cref="Id"/>:
/// duas instâncias com o mesmo Id representam a mesma coisa.
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    /// <summary>Guid versão 7: ordenado pelo tempo de criação, o que deixa os índices do banco mais eficientes.</summary>
    public Guid Id { get; private init; } = Guid.CreateVersion7();

    public bool Equals(Entity? other) =>
        other is not null && other.GetType() == GetType() && other.Id == Id;

    public override bool Equals(object? obj) => obj is Entity other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity? left, Entity? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);
}

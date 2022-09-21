using Repo.Abstractions.Interfaces;

namespace Repo.Abstractions.AbstractClasses;

public abstract class ModelWithId<TId> : IModelWithId<TId>, IEquatable<ModelWithId<TId>>
    where TId : IEquatable<TId>
{
    public TId Id { get; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is ModelWithId<TId> model) return Equals(model);
        return false;
    }

    public bool Equals(ModelWithId<TId>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public static bool operator ==(ModelWithId<TId> model1, ModelWithId<TId> model2)
    {
        return model1.Equals(model2);
    }

    public static bool operator !=(ModelWithId<TId> model1, ModelWithId<TId> model2)
    {
        return !model1.Equals(model2);
    }
}
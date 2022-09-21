namespace Repo.Abstractions.Interfaces;

public interface IModelWithId<TId> where TId : IEquatable<TId>
{
    TId Id { get; }
}
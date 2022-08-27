namespace Repo.Abstractions.Interfaces;

public interface IModel<TId> where TId : IEquatable<TId>
{
    TId Id { get; }
}
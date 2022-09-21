namespace Repo.Abstractions.Interfaces;

/// <summary>
/// Basic interface of the model with Id
/// </summary>
/// <typeparam name="TId">Type of model id</typeparam>
public interface IModelWithId<TId> where TId : IEquatable<TId>
{
    TId Id { get; }
}
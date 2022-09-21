namespace Repo.Abstractions.Interfaces;

/// <summary>
/// Basic repository interface
/// </summary>
/// <typeparam name="TModel">Model for repository</typeparam>
/// <typeparam name="TId"><inheritdoc cref="IModelWithId{TId}"/></typeparam>
public interface IRepository<TModel, TId> 
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Returns the model by Id
    /// </summary>
    /// <param name="id">Model id</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Model with chosen id or <see langword="null"/></returns>
    Task<TModel?> GetAsync(TId id, CancellationToken token = default);
    /// <summary>
    /// Returns the model by the selected property
    /// </summary>
    /// <param name="propertyName">Name of property <br/> Best practice is using <see langword="nameof"/></param>
    /// <param name="value">Value for chosen property</param>
    /// <param name="range">From which to which element to return</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <typeparam name="T">Type of property</typeparam>
    /// <returns><see cref="IList{T}"/> of models</returns>
    /// <example>
    /// This shows how to use it:
    /// <code>
    /// var models = await Repository.GetByAsync(nameof(User.Role), UserRoles.Admin, 5..10, token);
    /// </code>
    /// </example>
    Task<IList<TModel>> GetByAsync<T>(string propertyName, T value, Range range, CancellationToken token = default);
    /// <summary>
    /// Returns models by their IDs
    /// </summary>
    /// <param name="ids">Models IDs</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="IList{T}"/> of models</returns>
    Task<IList<TModel>> GetManyAsync(IList<TId> ids, CancellationToken token = default);
    /// <summary>
    /// Returns all models
    /// </summary>
    /// <param name="range">From which to which element to return</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>All currently available models</returns>
    Task<IList<TModel>> GetAllAsync(Range range, CancellationToken token = default);
    /// <summary>
    /// Creates an entity in the repository based on the passed model
    /// </summary>
    /// <param name="model">The model on the basis of which the entity will be created</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Id of the created entity</returns>
    Task<TId> CreateAsync(TModel model, CancellationToken token = default);
    /// <summary>
    /// Updates an entity in the repository based on the passed model
    /// </summary>
    /// <param name="id">Model id</param>
    /// <param name="model">The model based on which the entity will be updated</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    Task UpdateAsync(TId id, TModel model, CancellationToken token = default);
    /// <summary>
    /// Deletes entity by id
    /// </summary>
    /// <param name="id">Model id</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    Task DeleteAsync(TId id, CancellationToken token = default);
    /// <summary>
    /// Deletes entities by their IDs
    /// </summary>
    /// <param name="ids">Models IDs</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    Task DeleteManyAsync(IList<TId> ids, CancellationToken token = default);
}
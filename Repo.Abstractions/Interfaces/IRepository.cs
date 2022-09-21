using System.Linq.Expressions;

namespace Repo.Abstractions.Interfaces;

public interface IRepository<TModel, TId> 
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
{
    Task<TModel?> GetAsync(TId id, CancellationToken token = default);
    Task<IList<TModel>> GetByAsync<T>(string propertyName, T value, Range range, CancellationToken token = default);
    Task<IList<TModel>> GetManyAsync(IList<TId> ids, CancellationToken token = default);
    Task<IList<TModel>> GetAllAsync(CancellationToken token = default);
    Task<TId> CreateAsync(TModel model, CancellationToken token = default);
    Task UpdateAsync(TId id, TModel model, CancellationToken token = default);
    Task DeleteAsync(TId id, CancellationToken token = default);
    Task DeleteManyAsync(IList<TId> ids, CancellationToken token = default);
}
namespace Repo.Abstractions.Interfaces;

public interface IManager<TModel, TId> 
    where TModel : class, IModel<TId>
    where TId : IEquatable<TId>
{
    Task<TModel?> Get(TId id);
    Task<IList<TModel>> GetMany(params TId[] ids);
    Task<IList<TModel>> GetAll();
    Task<TId> Create(TModel model);
    Task Update(TId id, TModel model);
    Task Delete(TId id);
}
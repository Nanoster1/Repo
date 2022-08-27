using Repo.Abstractions.Interfaces;

namespace Repo.Abstractions.AbstractClasses;

public abstract class Manager<TModel, TId> : IManager<TModel, TId> 
    where TModel : class, IModel<TId>
    where TId : IEquatable<TId>
{
    protected readonly IValidator<TModel> Validator;
    protected readonly IRepository<TModel, TId> Repository;
    protected readonly IUnitOfWork UnitOfWork;

    protected Manager(IValidator<TModel> validator, IRepository<TModel, TId> repository, IUnitOfWork unitOfWork)
    {
        Validator = validator;
        Repository = repository;
        UnitOfWork = unitOfWork;
    }

    public virtual async Task<TId> Create(TModel model)
    {
        await Validator.ValidateAndThrowAsync(model);
        var id = await Repository.Create(model);
        await UnitOfWork.SaveChangesAsync();
        return id;
    }

    public virtual async Task Update(TId id, TModel model)
    {
        await Validator.ValidateAndThrowAsync(model);
        await Repository.Update(id, model);
        await UnitOfWork.SaveChangesAsync();
    }

    public virtual async Task Delete(TId id)
    {
        await Repository.Delete(id);
        await UnitOfWork.SaveChangesAsync();
    }

    public virtual async Task<TModel?> Get(TId id)
    {
        return await Repository.Get(id);
    }

    public virtual async Task<IList<TModel>> GetAll()
    {
        return await Repository.GetAll();
    }

    public virtual async Task<IList<TModel>> GetMany(params TId[] ids)
    {
        return await Repository.GetMany(ids);
    }
}
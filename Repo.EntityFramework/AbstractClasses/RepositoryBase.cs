using Microsoft.EntityFrameworkCore;
using Repo.EntityFramework.Interfaces;
using Repo.Abstractions.Interfaces;

namespace Repo.EntityFramework.AbstractClasses;

public abstract class RepositoryBase<TDbModel, TBsModel, TId> : IRepository<TBsModel, TId>
    where TDbModel : class, IModel<TId>, IRelatedDbModel<TDbModel, TBsModel>
    where TBsModel: class, IModel<TId>
    where TId : IEquatable<TId>
{
    protected readonly DbSet<TDbModel> Set;
    protected readonly DbContext Context;

    protected RepositoryBase(DbContext context)
    {
        Context = context;
        Set = Context.Set<TDbModel>();
    }

    public async Task<IList<TBsModel>> GetMany(params TId[] ids)
    {
        return await Set.AsNoTracking()
            .Where(dbModel => ids.Any(dbModel.Id.Equals))
            .Select(dbModel => dbModel.ConvertToBusinessModel())
            .ToListAsync();
    }

    public async Task<IList<TBsModel>> GetAll()
    {
        return await Set.AsNoTracking()
            .Select(dbModel => dbModel.ConvertToBusinessModel())
            .ToListAsync();
    }

    public async Task<TBsModel?> Get(TId id)
    {
        return await Set.Where(dbModel => dbModel.Id.Equals(id))
            .Select(dbModel => dbModel.ConvertToBusinessModel())
            .FirstOrDefaultAsync();
    }

    public async Task Update(TId id, TBsModel bsModel)
    {
        var dbModel = await GetDbModel(id, true);
        dbModel?.UpdateDataFromBusinessModel(bsModel);
    }

    public async Task Delete(TId id)
    {
        var dbModel = await GetDbModel(id, false);
        if (dbModel != null) Set.Remove(dbModel);
    }

    public async Task<TId> Create(TBsModel bsModel)
    {
        var dbModel = TDbModel.CreateFromBusinessModel(bsModel);
        await Set.AddAsync(dbModel);
        return dbModel.Id;
    }

    protected async Task<TDbModel?> GetDbModel(TId id, bool isTracking)
    {
        return await (isTracking ? Set.AsTracking() : Set.AsNoTracking())
            .Where(dbModel => dbModel.Id.Equals(id))
            .FirstOrDefaultAsync();
    }
}
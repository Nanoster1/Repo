using Microsoft.EntityFrameworkCore;
using Repo.EntityFramework.Interfaces;
using Repo.Abstractions.Interfaces;

namespace Repo.EntityFramework.AbstractClasses;

public abstract class RepositoryBase<TDbModel, TBsModel, TId> : IRepository<TBsModel, TId>
    where TDbModel : class, IModelWithId<TId>, IRelatedDbModel<TDbModel, TBsModel>
    where TBsModel: class, IModelWithId<TId>
    where TId : IEquatable<TId>
{
    protected readonly DbSet<TDbModel> Set;
    protected readonly DbContext Context;

    protected RepositoryBase(DbContext context)
    {
        Context = context;
        Set = context.Set<TDbModel>();
    }

    protected virtual async Task<TDbModel?> GetDbModelAsync(TId id, bool isTracking, CancellationToken token = default)
    {
        return await (isTracking ? Set.AsTracking() : Set.AsNoTracking())
            .Where(dbModel => dbModel.Id.Equals(id))
            .FirstOrDefaultAsync(token).ConfigureAwait(false);
    }

    public virtual async Task<TBsModel?> GetAsync(TId id, CancellationToken token = default)
    {
        var dbModel = await GetDbModelAsync(id, false, token).ConfigureAwait(false);
        return dbModel?.ConvertToBusinessModel();
    }

    public virtual async Task<IList<TBsModel>> GetByAsync<T>(string propertyName, T value, Range range, CancellationToken token = default)
    {
        var tableName = Set.EntityType.GetSchemaQualifiedTableName();
        if (tableName == null) throw new NullReferenceException($"{nameof(tableName)} is null");
        return await Set.FromSqlInterpolated($"SELECT * FROM {tableName} WHERE {propertyName} = {value}")
            .AsNoTracking()
            .Select(model => model.ConvertToBusinessModel())
            .ToListAsync(token).ConfigureAwait(false);
    }

    public virtual async Task<IList<TBsModel>> GetManyAsync(IList<TId> ids, CancellationToken token = default)
    {
        return await Set.AsNoTracking()
            .Where(dbModel => ids.Contains(dbModel.Id))
            .Select(dbModel => dbModel.ConvertToBusinessModel())
            .ToListAsync(token).ConfigureAwait(false);
    }

    public virtual async Task<IList<TBsModel>> GetAllAsync(Range range, CancellationToken token = default)
    {
        return await Set.AsNoTracking()
            .Select(dbModel => dbModel.ConvertToBusinessModel())
            .Take(range)
            .ToListAsync(token).ConfigureAwait(false);
    }

    public virtual async Task<TId> CreateAsync(TBsModel model, CancellationToken token = default)
    {
        var dbModel = TDbModel.CreateFromBusinessModel(model);
        await Set.AddAsync(dbModel, token).ConfigureAwait(false);
        return model.Id;
    }

    public virtual async Task UpdateAsync(TId id, TBsModel model, CancellationToken token = default)
    {
        var dbModel = await GetDbModelAsync(id, true, token).ConfigureAwait(false);
        dbModel?.UpdateDataFromBusinessModel(model);
    }

    public virtual async Task DeleteAsync(TId id, CancellationToken token = default)
    {
        var dbModel = await GetDbModelAsync(id, true, token); 
        if (dbModel != null) Set.Remove(dbModel);
    }

    public virtual async Task DeleteManyAsync(IList<TId> ids, CancellationToken token = default)
    {
        var dbModels = await Set.Where(model => ids.Contains(model.Id))
            .ToListAsync(token).ConfigureAwait(false);
        Set.RemoveRange(dbModels);
    }
}
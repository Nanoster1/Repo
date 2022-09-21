namespace Repo.Abstractions.Interfaces;

public interface ICrudManager<TModel, TId, TCreateDto, TUpdateDto, TGetDto>
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
    where TCreateDto : class
    where TUpdateDto : class
    where TGetDto : class
{
    Task<TGetDto?> GetAsync(TId id, CancellationToken token = default);
    Task<IList<TGetDto>> GetByAsync<T>(string propertyName, T value, Range range, CancellationToken token = default);
    Task<IList<TGetDto>> GetManyAsync(IList<TId> ids, CancellationToken token = default);
    Task<IList<TGetDto>> GetAllAsync(CancellationToken token = default);
    Task<TId> CreateAsync(TCreateDto createDto, CancellationToken token = default);
    Task UpdateAsync(TId id, TUpdateDto updateDto, CancellationToken token = default);
    Task DeleteAsync(TId id, CancellationToken token = default);
    Task DeleteManyAsync(IList<TId> ids, CancellationToken token = default);
}

public interface ICrudManager<TModel, TId, TCreateDto, TUpdateDto> : ICrudManager<TModel, TId, TCreateDto, TUpdateDto, TModel>
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
    where TCreateDto : class
    where TUpdateDto : class
{
}

public interface ICrudManager<TModel, TId, TCreateDto> : ICrudManager<TModel, TId, TCreateDto, TModel>
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
    where TCreateDto : class
{
}

public interface ICrudManager<TModel, TId> : ICrudManager<TModel, TId, TModel>
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
{
}
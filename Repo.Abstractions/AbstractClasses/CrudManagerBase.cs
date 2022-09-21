using Repo.Abstractions.Interfaces;

namespace Repo.Abstractions.AbstractClasses;

public abstract class CrudManagerBase<TModel, TId, TCreateDto, TUpdateDto, TGetDto> : 
    ICrudManager<TModel, TId, TCreateDto, TUpdateDto, TGetDto> 
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
    where TCreateDto : class
    where TUpdateDto : class
    where TGetDto : class
{
    protected readonly IRepository<TModel, TId> Repository;
    protected readonly IObjectMapper? ObjectMapper;
    protected readonly IUnitOfWork UnitOfWork;
    
    protected CrudManagerBase(IRepository<TModel, TId> repository, IUnitOfWork unitOfWork, IObjectMapper? objectMapper = null)
    {
        Repository = repository;
        ObjectMapper = objectMapper;
        UnitOfWork = unitOfWork;
    }

    #region Get
    
    public virtual async Task<TGetDto?> GetAsync(TId id, CancellationToken token = default)
    {
        var isCanceled = await OnBeforeGetAsync(id, token).ConfigureAwait(false);
        if (isCanceled) return null;
        var model = await Repository.GetAsync(id, token).ConfigureAwait(false);
        if (model == null) return null;
        var dto = OnCreateDtoInGetAsync(model);
        await OnAfterGetAsync(id, dto, token).ConfigureAwait(false);
        return dto;
    }

    protected virtual Task<bool> OnBeforeGetAsync(TId id, CancellationToken token) => Task.FromResult(false);
    protected virtual Task OnAfterGetAsync(TId id, TGetDto getDto, CancellationToken token) => Task.CompletedTask;
    protected virtual TGetDto OnCreateDtoInGetAsync(TModel model) => ObjectMapper != null 
        ? ObjectMapper.CreateAndMap<TModel, TGetDto>(model)
        : throw new NullReferenceException($"{nameof(ObjectMapper)} is null");
    
    #endregion

    #region GetBy

    public virtual async Task<IList<TGetDto>> GetByAsync<T>(string propertyName, T value, Range range, CancellationToken token = default)
    {
        var isCanceled = await OnBeforeGetByAsync(propertyName, value, range, token).ConfigureAwait(false);
        if (isCanceled) return Array.Empty<TGetDto>();
        var models = await Repository.GetByAsync(propertyName, value, range, token).ConfigureAwait(false);
        var dto = OnCreateDtoInGetByAsync(models); 
        await OnAfterGetByAsync(propertyName, value, range, dto, token);
        return dto;
    }

    protected virtual Task<bool> OnBeforeGetByAsync<T>(string propertyName, T value, Range range, CancellationToken token) => Task.FromResult(false);
    protected virtual Task OnAfterGetByAsync<T>(string propertyName, T value, Range range, IList<TGetDto> getDto, CancellationToken token) => Task.CompletedTask;
    protected virtual IList<TGetDto> OnCreateDtoInGetByAsync(IList<TModel> models) => ObjectMapper != null 
        ? models.Select(model => ObjectMapper.CreateAndMap<TModel, TGetDto>(model)).ToList()
        : throw new NullReferenceException($"{nameof(ObjectMapper)} is null");

    
    #endregion

    #region GetMany

    public virtual async Task<IList<TGetDto>> GetManyAsync(IList<TId> ids, CancellationToken token = default)
    {
        var isCanceled = await OnBeforeGetManyAsync(ids, token).ConfigureAwait(false);
        if (isCanceled) return Array.Empty<TGetDto>();
        var models = await Repository.GetManyAsync(ids, token).ConfigureAwait(false);
        var dto = OnCreateDtoInGetManyAsync(models);
        await OnAfterGetManyAsync(ids, dto, token).ConfigureAwait(false);
        return dto;
    }

    protected virtual Task<bool> OnBeforeGetManyAsync(IList<TId> ids, CancellationToken token) => Task.FromResult(false);
    protected virtual Task OnAfterGetManyAsync(IList<TId> ids, IList<TGetDto> getDto, CancellationToken token) => Task.CompletedTask;
    protected virtual IList<TGetDto> OnCreateDtoInGetManyAsync(IList<TModel> models) => ObjectMapper != null 
        ? models.Select(model => ObjectMapper.CreateAndMap<TModel, TGetDto>(model)).ToList()
        : throw new NullReferenceException($"{nameof(ObjectMapper)} is null");

    
    #endregion

    #region GetAll

    public virtual async Task<IList<TGetDto>> GetAllAsync(CancellationToken token = default)
    {
        var isCanceled = await OnBeforeGetAllAsync(token).ConfigureAwait(false);
        if (isCanceled) return Array.Empty<TGetDto>();
        var models = await Repository.GetAllAsync(token).ConfigureAwait(false);
        var dto = OnCreateDtoInGetAllAsync(models);
        await OnAfterGetAllAsync(dto, token).ConfigureAwait(false);
        return dto;
    }
    
    protected virtual Task<bool> OnBeforeGetAllAsync(CancellationToken token) => Task.FromResult(false);
    protected virtual Task OnAfterGetAllAsync(IList<TGetDto> getDto, CancellationToken token) => Task.CompletedTask;
    protected virtual IList<TGetDto> OnCreateDtoInGetAllAsync(IList<TModel> models) => ObjectMapper != null 
        ? models.Select(model => ObjectMapper.CreateAndMap<TModel, TGetDto>(model)).ToList()
        : throw new NullReferenceException($"{nameof(ObjectMapper)} is null");

    
    #endregion

    #region Create

    public virtual async Task<TId> CreateAsync(TCreateDto createDto, CancellationToken token = default)
    {
        await OnBeforeCreateAsync(createDto, token).ConfigureAwait(false);
        var model = OnCreateModelInCreateAsync(createDto);
        var id = await Repository.CreateAsync(model, token).ConfigureAwait(false);
        var saveChangesResult = await UnitOfWork.SaveChangesAsync(token).ConfigureAwait(false);
        await OnAfterCreateAsync(createDto, id, model, saveChangesResult, token).ConfigureAwait(false);
        return id;
    }

    protected virtual Task OnBeforeCreateAsync(TCreateDto createDto, CancellationToken token) => Task.CompletedTask;
    protected virtual Task OnAfterCreateAsync(TCreateDto createDto, TId id, TModel model, int saveChangesResult, CancellationToken token) => Task.CompletedTask;
    protected virtual TModel OnCreateModelInCreateAsync(TCreateDto createDto) => ObjectMapper != null
        ? ObjectMapper.CreateAndMap<TCreateDto, TModel>(createDto)
        : throw new NullReferenceException();

    #endregion

    #region Update

    public virtual async Task UpdateAsync(TId id, TUpdateDto updateDto, CancellationToken token = default)
    {
        var isCanceled = await OnBeforeUpdateAsync(id, updateDto, token).ConfigureAwait(false);
        if (isCanceled) return;
        var model = OnCreateModelInUpdateAsync(id, updateDto);
        await Repository.UpdateAsync(id, model, token).ConfigureAwait(false);
        var saveChangesResult = await UnitOfWork.SaveChangesAsync(token).ConfigureAwait(false);
        await OnAfterUpdateAsync(id, updateDto, model, saveChangesResult, token).ConfigureAwait(false);
    }
    
    protected virtual Task<bool> OnBeforeUpdateAsync(TId id, TUpdateDto updateDto, CancellationToken token) => Task.FromResult(false);
    protected virtual Task OnAfterUpdateAsync(TId id, TUpdateDto updateDto, TModel model, int saveChangesResult ,CancellationToken token) => Task.CompletedTask;

    protected virtual TModel OnCreateModelInUpdateAsync(TId id, TUpdateDto updateDto) => ObjectMapper != null
        ? ObjectMapper.CreateAndMap<TUpdateDto, TModel>(updateDto) 
        : throw new NullReferenceException();

    #endregion

    #region Delete
    
    public virtual async Task DeleteAsync(TId id, CancellationToken token = default)
    {
        var isCanceled = await OnBeforeDeleteAsync(id, token).ConfigureAwait(false);
        if (isCanceled) return;
        await Repository.DeleteAsync(id, token).ConfigureAwait(false);
        var saveChangesResult = await UnitOfWork.SaveChangesAsync(token).ConfigureAwait(false);
        await OnAfterDeleteAsync(id, saveChangesResult, token).ConfigureAwait(false);
    }

    protected virtual Task<bool> OnBeforeDeleteAsync(TId id, CancellationToken token) => Task.FromResult(false);
    protected virtual Task OnAfterDeleteAsync(TId id, int saveChangesResult, CancellationToken token) => Task.CompletedTask;
    
    #endregion

    #region DeleteMany

    public virtual async Task DeleteManyAsync(IList<TId> ids, CancellationToken token = default)
    {
        var isCanceled = await OnBeforeDeleteManyAsync(ids, token).ConfigureAwait(false);
        if (isCanceled) return;
        await Repository.DeleteManyAsync(ids, token).ConfigureAwait(false);
        var saveChangesResult = await UnitOfWork.SaveChangesAsync(token).ConfigureAwait(false);
        await OnAfterDeleteManyAsync(ids, saveChangesResult, token).ConfigureAwait(false);
    }

    protected virtual Task<bool> OnBeforeDeleteManyAsync(IList<TId> ids, CancellationToken token) => Task.FromResult<bool>(false);
    protected virtual Task OnAfterDeleteManyAsync(IList<TId> ids, int saveChangesResult, CancellationToken token) => Task.FromResult(false);

    #endregion
}

#region CrudManagerBaseRealization2

public abstract class CrudManagerBase<TModel, TId, TCreateDto, TUpdateDto> :
    CrudManagerBase<TModel, TId, TCreateDto, TUpdateDto, TModel>,
    ICrudManager<TModel, TId, TCreateDto, TUpdateDto>
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
    where TCreateDto : class
    where TUpdateDto : class
{
    protected CrudManagerBase(IRepository<TModel, TId> repository, IUnitOfWork unitOfWork, IObjectMapper? objectMapper = null) : base(repository, unitOfWork, objectMapper)
    {
    }

    protected override TModel OnCreateDtoInGetAsync(TModel model) => model;
    protected override IList<TModel> OnCreateDtoInGetByAsync(IList<TModel> models) => models;
    protected override IList<TModel> OnCreateDtoInGetAllAsync(IList<TModel> models) => models;
    protected override IList<TModel> OnCreateDtoInGetManyAsync(IList<TModel> models) => models;
}

#endregion

#region CrudManagerBaseRealization3

public abstract class CrudManagerBase<TModel, TId, TCreateDto> :
    CrudManagerBase<TModel, TId, TCreateDto, TModel>,
    ICrudManager<TModel, TId, TCreateDto>
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
    where TCreateDto : class
{
    protected CrudManagerBase(IRepository<TModel, TId> repository, IUnitOfWork unitOfWork, IObjectMapper? objectMapper = null) : base(repository, unitOfWork, objectMapper)
    {
    }

    protected override TModel OnCreateModelInUpdateAsync(TId id, TModel model) => model;
}

#endregion

#region CrudManagerBaseRealization4

public abstract class CrudManagerBase<TModel, TId> :
    CrudManagerBase<TModel, TId, TModel>,
    ICrudManager<TModel, TId>
    where TModel : class, IModelWithId<TId>
    where TId : IEquatable<TId>
{
    protected CrudManagerBase(IRepository<TModel, TId> repository, IUnitOfWork unitOfWork, IObjectMapper? objectMapper = null) : base(repository, unitOfWork, objectMapper)
    {
    }

    protected override TModel OnCreateModelInCreateAsync(TModel model) => model;
}

#endregion
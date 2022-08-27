namespace Repo.Abstractions.Interfaces;

public interface IValidator<TModel>
{
    bool Validate(TModel model);
    Task<bool> ValidateAsync(TModel model);
    void ValidateAndThrow(TModel model);
    Task ValidateAndThrowAsync(TModel model);
}
using Repo.Abstractions.AbstractClasses;

namespace Repo.Abstractions.Interfaces;

/// <summary>
/// Basic interface for validators
/// </summary>
/// <typeparam name="TModel">Model for validating</typeparam>
public interface IValidator<TModel>
{
    /// <summary>
    /// Validates the model
    /// </summary>
    /// <param name="model">Model for validating</param>
    /// <returns>Is the model valid</returns>
    bool Validate(TModel model);
    /// <summary>
    /// Validates the model asynchronously
    /// </summary>
    /// <param name="model">Model for validating</param>
    /// <returns>Is the model valid</returns>
    Task<bool> ValidateAsync(TModel model, CancellationToken token = default);
    /// <summary>
    /// Validates the model and throws exception if model is not valid 
    /// </summary>
    /// <param name="model">Model for validating</param>
    /// <exception cref="ValidationExceptionBase">Some validation exception</exception>
    void ValidateAndThrow(TModel model);
    /// <summary>
    /// Validates the model and throws exception if model is not valid asynchronously
    /// </summary>
    /// <param name="model">Model for validating</param>
    /// <exception cref="ValidationExceptionBase">Some validation exception</exception>
    Task ValidateAndThrowAsync(TModel model, CancellationToken token = default);
}
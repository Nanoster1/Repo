namespace Repo.Abstractions.Interfaces;

/// <summary>
/// The basic ObjectMapper interface
/// </summary>
public interface IObjectMapper
{
    /// <summary>
    /// Creates <see cref="TObjectTo"/> object and map <see cref="TObjectFrom"/> object to it
    /// </summary>
    /// <param name="objectFrom">The object to be mapped</param>
    /// <typeparam name="TObjectFrom">Type of the base object</typeparam>
    /// <typeparam name="TObjectTo">Type of the mapped object</typeparam>
    /// <returns>New mapped object</returns>
    TObjectTo CreateAndMap<TObjectFrom, TObjectTo>(TObjectFrom objectFrom);
    /// <summary>
    /// Maps one object to another
    /// </summary>
    /// <inheritdoc cref="CreateAndMap{TObjectFrom,TObjectTo}"/>
    /// <param name="objectTo">Mapped object</param>
    /// <returns></returns>
    void MapTo<TObjectFrom, TObjectTo>(TObjectFrom objectFrom, TObjectTo objectTo);
    /// <summary>
    /// Tries to create <see cref="TObjectTo"/> object and map <see cref="TObjectFrom"/> object to it
    /// </summary>
    /// <inheritdoc cref="CreateAndMap{TObjectFrom,TObjectTo}"/>
    /// <returns>Is mapped</returns>
    bool TryCreateAndMap<TObjectFrom, TObjectTo>(TObjectFrom objectFrom, out TObjectTo objectTo);
    /// <summary>
    /// Tries to map one object to another
    /// </summary>
    /// <inheritdoc cref="MapTo{TObjectFrom,TObjectTo}"/>
    /// <returns>Is mapped</returns>
    bool TryMapTo<TObjectFrom, TObjectTo>(TObjectFrom objectFrom, TObjectTo objectTo);
}
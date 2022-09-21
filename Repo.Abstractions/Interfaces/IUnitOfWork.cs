namespace Repo.Abstractions.Interfaces;

/// <summary>
/// Interface for encapsulating data saving in a database
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves changes to the database
    /// </summary>
    int SaveChanges();
    /// <summary>
    /// Saves changes to the database asynchronously
    /// </summary>
    /// <param name="token"></param>
    Task<int> SaveChangesAsync(CancellationToken token = default);
}
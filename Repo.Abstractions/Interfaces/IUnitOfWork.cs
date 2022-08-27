namespace Repo.Abstractions.Interfaces;

public interface IUnitOfWork
{
    int SaveChanges();
    Task<int> SaveChangesAsync();
}
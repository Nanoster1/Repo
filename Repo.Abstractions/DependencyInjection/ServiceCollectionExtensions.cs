using Microsoft.Extensions.DependencyInjection;
using Repo.Abstractions.Interfaces;

namespace Repo.Abstractions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface
        where TInterface : class
    {
        var repositoryInterface = typeof(TImplementation).GetInterfaces()
            .Where(i => i.IsGenericType)
            .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IRepository<,>));

        if (repositoryInterface == null)
            throw new InvalidOperationException($"{nameof(TImplementation)} don't implements IRepository");
        
        services.AddScoped(repositoryInterface, typeof(TImplementation));
        services.AddScoped<TInterface, TImplementation>();
        
        return services;
    }
}
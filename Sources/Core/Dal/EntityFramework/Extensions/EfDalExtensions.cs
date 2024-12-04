using System;
using Core.Dal.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Dal.EntityFramework.Extensions;

public static class EfDalExtensions
{
    public static IServiceCollection AddDbContextFactory<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction)
        where TContext : DbContext =>
        services.AddSingleton<IDbContextFactory>(provider =>
        {
            var builder = new DbContextOptionsBuilder<TContext>(new DbContextOptions<TContext>());
            optionsAction?.Invoke(builder);

            return new DbContextFactory<TContext>(() => ActivatorUtilities.CreateInstance<TContext>(provider, builder.Options));
        });

    public static IServiceCollection AddRepository<TService, TImplementation>(this IServiceCollection services)
        where TService : IRepository
        where TImplementation : EfRepository, TService
    {
        return services.AddSingleton<EfRepositoryFactory<TService>>(Factory);

        static EfRepositoryFactory<TImplementation> Factory(IServiceProvider provider) =>
            context => ActivatorUtilities.CreateInstance<TImplementation>(provider, context);
    }
}

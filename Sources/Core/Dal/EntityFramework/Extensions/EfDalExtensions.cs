using System;
using Core.Dal.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Dal.EntityFramework.Extensions;

public static class EfDalExtensions
{
    public static IServiceCollection AddEfUnitOfWorkFactory<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder<TContext>>? optionsAction)
        where TContext : DbContext =>
        services
            .AddSingleton<IUnitOfWorkFactory, EfUnitOfWorkFactory>()
            .AddSingleton<IDbContextFactory>(provider =>
            {
                var builder = new DbContextOptionsBuilder<TContext>();
                optionsAction?.Invoke(builder);

                return new DbContextFactory<TContext>(() => ActivatorUtilities.CreateInstance<TContext>(provider, builder.Options));
            });

    public static IServiceCollection AddRepository<TService, TImplementation>(this IServiceCollection services)
        where TService : IRepository
        where TImplementation : EfRepository, TService =>
        services.AddSingleton<EfRepository.Factory<TService>>(provider =>
            context => ActivatorUtilities.CreateInstance<TImplementation>(provider, context));
}

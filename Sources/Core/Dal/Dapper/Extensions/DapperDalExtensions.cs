using Core.Dal.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Dal.Dapper.Extensions;

public static class DapperDalExtensions
{
    public static IServiceCollection AddDapperUnitOfWorkFactory(this IServiceCollection services) =>
        services.AddSingleton<IUnitOfWorkFactory, DapperUnitOfWorkFactory>();

    public static IServiceCollection AddRepository<TService, TImplementation>(this IServiceCollection services)
        where TService : IRepository
        where TImplementation : DapperRepository, TService =>
        services.AddSingleton<DapperRepository.Factory<TService>>(provider =>
            (connection, transaction) => ActivatorUtilities.CreateInstance<TImplementation>(provider, connection, transaction));
}

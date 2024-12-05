using Microsoft.Extensions.DependencyInjection;

namespace Core.Dal.Extensions;

public static class DalExtensions
{
    public static IServiceCollection AddUnitOfWorkFactory<TUnitOfWorkFactory>(this IServiceCollection services)
        where TUnitOfWorkFactory : class, IUnitOfWorkFactory =>
        services.AddSingleton<IUnitOfWorkFactory, TUnitOfWorkFactory>();
}

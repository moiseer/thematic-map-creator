using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Dal.EntityFramework;

public sealed class EfUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory _contextFactory;
    private readonly IServiceProvider _serviceProvider;

    public EfUnitOfWorkFactory(IDbContextFactory contextFactory, IServiceProvider serviceProvider)
    {
        _contextFactory = contextFactory;
        _serviceProvider = serviceProvider;
    }

    public IUnitOfWork Create() => new EfUnitOfWork(_contextFactory.Create(), _serviceProvider);

    public Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Create());
    }
}

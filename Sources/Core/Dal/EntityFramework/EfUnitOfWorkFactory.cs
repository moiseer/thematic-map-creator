using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Dal.EntityFramework;

public sealed class EfUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory contextFactory;
    private readonly IServiceProvider serviceProvider;

    public EfUnitOfWorkFactory(IDbContextFactory contextFactory, IServiceProvider serviceProvider)
    {
        this.contextFactory = contextFactory;
        this.serviceProvider = serviceProvider;
    }

    public IUnitOfWork Create() => new EfUnitOfWork(contextFactory.Create(), serviceProvider);

    public Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Create());
    }
}

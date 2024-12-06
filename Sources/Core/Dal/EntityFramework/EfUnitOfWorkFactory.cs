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

    /// <inheritdoc/>
    public IUnitOfWork Create()
    {
        var context = _contextFactory.Create();
        var transaction = context.Database.BeginTransaction();
        return new EfUnitOfWork(context, transaction, _serviceProvider);
    }

    /// <inheritdoc/>
    public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
    {
        var context = _contextFactory.Create();
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new EfUnitOfWork(context, transaction, _serviceProvider);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework;

public sealed class EfUnitOfWorkFactory<TContext> : IUnitOfWorkFactory
    where TContext : DbContext
{
    private readonly IDbContextFactory<TContext> _contextFactory;
    private readonly IServiceProvider _serviceProvider;

    public EfUnitOfWorkFactory(IDbContextFactory<TContext> contextFactory, IServiceProvider serviceProvider)
    {
        _contextFactory = contextFactory;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public IUnitOfWork Create()
    {
        var context = _contextFactory.CreateDbContext();
        var transaction = context.Database.BeginTransaction();
        return new EfUnitOfWork(context, transaction, _serviceProvider);
    }

    /// <inheritdoc/>
    public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
    {
        var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new EfUnitOfWork(context, transaction, _serviceProvider);
    }
}

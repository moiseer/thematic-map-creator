using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;

namespace Core.Dal.EntityFramework;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly AsyncLock _locker = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextTransaction _transaction;
    private bool _committed;

    public EfUnitOfWork(DbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _transaction = context.Database.BeginTransaction();
    }

    public void Commit()
    {
        using (_locker.Lock())
        {
            _context.SaveChanges();
            _transaction.Commit();
            _committed = true;
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        using (await _locker.LockAsync(cancellationToken))
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
            _committed = true;
        }
    }

    public void Dispose()
    {
        using (_locker.Lock())
        {
            if (!_committed)
            {
                _transaction.Rollback();
            }

            _transaction.Dispose();
            _context.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        using (await _locker.LockAsync())
        {
            if (!_committed)
            {
                await _transaction.RollbackAsync();
            }

            await _transaction.DisposeAsync();
            await _context.DisposeAsync();
        }
    }

    public TRepository GetRepository<TRepository>()
        where TRepository : IRepository =>
        _serviceProvider.GetRequiredService<EfRepositoryFactory<TRepository>>().Invoke(_context);
}

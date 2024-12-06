using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;

namespace Core.Dal.Dapper;

public sealed class DapperUnitOfWork : IUnitOfWork
{
    private readonly DbConnection _connection;
    private readonly AsyncLock _locker = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly DbTransaction _transaction;
    private bool _committed;

    public DapperUnitOfWork(DbConnection connection, DbTransaction transaction, IServiceProvider serviceProvider)
    {
        _connection = connection;
        _transaction = transaction;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public void Commit()
    {
        using (_locker.Lock())
        {
            _transaction.Commit();
            _committed = true;
        }
    }

    /// <inheritdoc/>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        using (await _locker.LockAsync(cancellationToken))
        {
            await _transaction.CommitAsync(cancellationToken);
            _committed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        using (_locker.Lock())
        {
            if (!_committed)
            {
                _transaction.Rollback();
            }

            _transaction.Dispose();
            _connection.Dispose();
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        using (await _locker.LockAsync())
        {
            if (!_committed)
            {
                await _transaction.RollbackAsync();
            }

            await _transaction.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }

    /// <inheritdoc/>
    public TRepository GetRepository<TRepository>()
        where TRepository : IRepository =>
        _serviceProvider.GetRequiredService<DapperRepository.Factory<TRepository>>().Invoke(_connection, _transaction);
}

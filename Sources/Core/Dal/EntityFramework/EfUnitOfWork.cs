﻿using System;
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
    private readonly DbContext context;
    private readonly AsyncLock locker = new();
    private readonly IServiceProvider serviceProvider;
    private readonly IDbContextTransaction transaction;
    private bool committed;

    public EfUnitOfWork(DbContext context, IServiceProvider serviceProvider)
    {
        this.context = context;
        this.serviceProvider = serviceProvider;
        transaction = context.Database.BeginTransaction();
    }

    public void Commit()
    {
        using (locker.Lock())
        {
            context.SaveChanges();
            transaction.Commit();
            committed = true;
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        using (await locker.LockAsync(cancellationToken))
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            committed = true;
        }
    }

    public void Dispose()
    {
        using (locker.Lock())
        {
            if (!committed)
            {
                transaction.Rollback();
            }

            transaction.Dispose();
            context.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        using (await locker.LockAsync())
        {
            if (!committed)
            {
                await transaction.RollbackAsync();
            }

            await transaction.DisposeAsync();
            await context.DisposeAsync();
        }
    }

    public TRepository GetRepository<TRepository>()
        where TRepository : IRepository
    {
        return serviceProvider.GetRequiredService<EfRepositoryFactory<TRepository>>().Invoke(context);
    }
}

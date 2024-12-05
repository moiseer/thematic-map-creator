using System;
using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework;

public class DbContextFactory<TContext> : IDbContextFactory
    where TContext : DbContext
{
    private readonly Func<TContext> _innerFactory;

    public DbContextFactory(Func<TContext> innerFactory) => _innerFactory = innerFactory;

    public TContext Create() => _innerFactory.Invoke();
    DbContext IDbContextFactory.Create() => Create();
}

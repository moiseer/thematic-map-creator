using System;
using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework;

public class DbContextFactory<TContext> : IDbContextFactory
    where TContext : DbContext
{
    private readonly Func<TContext> innerFactory;

    public DbContextFactory(Func<TContext> innerFactory) => this.innerFactory = innerFactory;

    public TContext Create() => innerFactory.Invoke();
    DbContext IDbContextFactory.Create() => Create();
}

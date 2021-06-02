using System;
using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework
{
    public class DbContextFactory<TContext> : IDbContextFactory
        where TContext : DbContext
    {
        private readonly Func<TContext> innerFactory;

        public DbContextFactory(string tag, Func<TContext> innerFactory)
        {
            Tag = tag;
            this.innerFactory = innerFactory;
        }

        public string Tag { get; }
        public TContext Create() => innerFactory.Invoke();
        DbContext IDbContextFactory.Create() => Create();
    }
}

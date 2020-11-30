using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Dal.EntityFramework
{
    public class EfUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IEnumerable<IDbContextFactory> contextFactories;
        private readonly IServiceProvider serviceProvider;

        public EfUnitOfWorkFactory(IEnumerable<IDbContextFactory> contextFactories, IServiceProvider serviceProvider)
        {
            this.contextFactories = contextFactories;
            this.serviceProvider = serviceProvider;
        }

        public IUnitOfWork Create()
        {
            var contextFactory = contextFactories.Single();
            return Create(contextFactory);
        }

        public IUnitOfWork Create(string tag)
        {
            var contextFactory = contextFactories.Single(factory => factory.Tag == tag);
            return Create(contextFactory);
        }

        public Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Create());
        }

        public Task<IUnitOfWork> CreateAsync(string tag, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Create(tag));
        }

        private IUnitOfWork Create(IDbContextFactory contextFactory) =>
            new EfUnitOfWork(contextFactory.Tag, contextFactory.Create(), serviceProvider);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.Repositories;

namespace Core.Dal
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        string Tag { get; }
        void Commit();
        Task CommitAsync(CancellationToken cancellationToken = default);

        TRepository GetRepository<TRepository>()
            where TRepository : IRepository;
    }
}

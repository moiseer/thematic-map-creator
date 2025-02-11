using Core.Dal.Repositories;

namespace Core.Dal;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    void Commit();
    Task CommitAsync(CancellationToken cancellationToken = default);

    TRepository GetRepository<TRepository>()
        where TRepository : IRepository;
}

using Core.Dal.Models;

namespace Core.Dal.Repositories;

public interface IAsyncCrudRepository<TEntity, in TKey> : IRepository
    where TEntity : IEntity<TKey>
{
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TKey id, CancellationToken ct = default);
    Task DeleteAsync(IEnumerable<TKey> ids, CancellationToken ct = default);
    Task<TEntity?> GetAsync(TKey id, CancellationToken ct = default);
    Task<List<TEntity>> GetAsync(IEnumerable<TKey> ids, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.Models;

namespace Core.Dal.Repositories
{
    public interface IAsyncCrudRepository<TEntity, in TKey> : IRepository
        where TEntity : IEntity<TKey>
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        Task DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}

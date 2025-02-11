using Core.Dal.Models;

namespace Core.Dal.Repositories;

public interface ICrudRepository<TEntity, in TKey> : IRepository
    where TEntity : IEntity<TKey>
{
    void Add(TEntity entity);
    void Delete(TKey id);
    void Delete(IEnumerable<TKey> ids);
    TEntity? Get(TKey id);
    List<TEntity> Get(IEnumerable<TKey> ids);
    void Update(TEntity entity);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.Models;
using Core.Dal.Repositories;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Core.Dal.EntityFramework
{
    public abstract class EfCrudRepository<TEntity, TKey> :
        EfRepository,
        IAsyncCrudRepository<TEntity, TKey>,
        ICrudRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected EfCrudRepository(DbContext context) : base(context)
        {
        }

        public void Add(TEntity entity) =>
            Context.Set<TEntity>().Add(entity);

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            await Context.Set<TEntity>().AddAsync(entity, cancellationToken);

        public void Delete(TKey id) =>
            Context.Set<TEntity>().Where(entity => id.Equals(entity.Id)).Delete();

        public void Delete(IEnumerable<TKey> ids) =>
            Context.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).Delete();

        public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default) =>
            await Context.Set<TEntity>().Where(entity => id.Equals(entity.Id)).DeleteAsync(cancellationToken);

        public async Task DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) =>
            await Context.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).DeleteAsync(cancellationToken);

        public TEntity Get(TKey id) =>
            Context.Set<TEntity>().FirstOrDefault(entity => id.Equals(entity.Id));

        public List<TEntity> Get(IEnumerable<TKey> ids) =>
            Context.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).ToList();

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default) =>
            await Context.Set<TEntity>().FirstOrDefaultAsync(entity => id.Equals(entity.Id), cancellationToken);

        public async Task<List<TEntity>> GetAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) =>
            await Context.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).ToListAsync(cancellationToken);

        public void Update(TEntity entity) =>
            Context.Set<TEntity>().Update(entity);

        public abstract Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}

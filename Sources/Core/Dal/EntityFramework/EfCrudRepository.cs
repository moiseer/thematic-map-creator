using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.Models;
using Core.Dal.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework;

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

    public async Task AddAsync(TEntity entity, CancellationToken ct = default) =>
        await Context.Set<TEntity>().AddAsync(entity, ct);

    public void Delete(TKey id) =>
        Context.Set<TEntity>().Where(entity => id.Equals(entity.Id)).ExecuteDelete();

    public void Delete(IEnumerable<TKey> ids) =>
        Context.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).ExecuteDelete();

    public async Task DeleteAsync(TKey id, CancellationToken ct = default) =>
        await Context.Set<TEntity>().Where(entity => id.Equals(entity.Id)).ExecuteDeleteAsync(ct);

    public async Task DeleteAsync(IEnumerable<TKey> ids, CancellationToken ct = default) =>
        await Context.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).ExecuteDeleteAsync(ct);

    public TEntity? Get(TKey id) =>
        Context.Set<TEntity>().FirstOrDefault(entity => id.Equals(entity.Id));

    public List<TEntity> Get(IEnumerable<TKey> ids) =>
        Context.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).ToList();

    public async Task<TEntity?> GetAsync(TKey id, CancellationToken ct = default) =>
        await Context.Set<TEntity>().FirstOrDefaultAsync(entity => id.Equals(entity.Id), ct);

    public async Task<List<TEntity>> GetAsync(IEnumerable<TKey> ids, CancellationToken ct = default) =>
        await Context.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).ToListAsync(ct);

    public void Update(TEntity entity) =>
        Context.Set<TEntity>().Update(entity);

    public abstract Task UpdateAsync(TEntity entity, CancellationToken ct = default);
}

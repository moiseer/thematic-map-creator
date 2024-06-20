using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Host.Persistence.Repositories;

public sealed class MapsRepository : EfCrudRepository<Map, Guid>, IMapsRepository
{
    public MapsRepository(DbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid userId, string name) =>
        await Context.Set<Map>().AnyAsync(map => map.UserId == userId && map.Name == name);

    public async Task<List<Map>> GetByUserIdAsync(Guid userId) =>
        await Context.Set<Map>().Where(map => map.UserId == userId).ToListAsync();

    public override async Task UpdateAsync(Map entity, CancellationToken ct = default) =>
        await Context.Set<Map>().Where(map => map.Id == entity.Id).ExecuteUpdateAsync(
            calls => calls
                .SetProperty(map => map.Name, entity.Name)
                .SetProperty(map => map.Description, entity.Description),
            ct);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using Z.EntityFramework.Plus;

namespace ThematicMapCreator.Host.Persistence.Repositories
{
    public class MapsRepository : EfCrudRepository<Map, Guid>, IMapsRepository
    {
        public MapsRepository(DbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(Guid userId, string name) =>
            await Context.Set<Map>().Where(map => map.UserId == userId).AnyAsync(map => map.Name == name);

        public async Task<List<Map>> GetByUserIdAsync(Guid userId) =>
            await Context.Set<Map>().Where(map => map.UserId == userId).ToListAsync();

        public override async Task UpdateAsync(Map entity, CancellationToken cancellationToken = default) =>
            await Context.Set<Map>().Where(map => map.Id == entity.Id).UpdateAsync(
                layer => new Map
                {
                    Name = entity.Name,
                    Description = entity.Description
                },
                cancellationToken);
    }
}

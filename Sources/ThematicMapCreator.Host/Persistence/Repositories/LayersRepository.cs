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
    public class LayersRepository : EfCrudRepository<Layer, Guid>, ILayersRepository
    {
        public LayersRepository(DbContext context) : base(context)
        {
        }

        public async Task AddAsync(IEnumerable<Layer> layers) =>
            await Context.Set<Layer>().AddRangeAsync(layers);

        public async Task DeleteByMapIdAsync(Guid mapId, IEnumerable<Guid> excludedLayerIds) =>
            await Context.Set<Layer>()
                .Where(layer => layer.MapId == mapId)
                .Where(layer => !excludedLayerIds.Contains(layer.Id))
                .DeleteAsync();

        public async Task<List<Layer>> GetByMapIdAsync(Guid mapId) =>
            await Context.Set<Layer>().Where(layer => layer.MapId == mapId).ToListAsync();

        public override async Task UpdateAsync(Layer entity, CancellationToken cancellationToken = default) =>
            await Context.Set<Layer>().Where(layer => layer.Id == entity.Id).UpdateAsync(
                layer => new Layer
                {
                    Name = entity.Name,
                    Description = entity.Description,
                    Index = entity.Index,
                    IsVisible = entity.IsVisible,
                    MapId = entity.MapId,
                    Data = entity.Data,
                    StyleOptions = entity.StyleOptions,
                    Type = entity.Type
                },
                cancellationToken);
    }
}

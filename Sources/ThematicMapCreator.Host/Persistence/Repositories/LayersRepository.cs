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

public sealed class LayersRepository : EfCrudRepository<Layer, Guid>, ILayersRepository
{
    public LayersRepository(DbContext context)
        : base(context)
    {
    }

    public async Task AddAsync(IEnumerable<Layer> layers) =>
        await Context.Set<Layer>().AddRangeAsync(layers);

    public async Task DeleteByMapIdAsync(Guid mapId, IEnumerable<Guid> excludedLayerIds) =>
        await Context.Set<Layer>()
            .Where(layer => layer.MapId == mapId)
            .Where(layer => !excludedLayerIds.Contains(layer.Id))
            .ExecuteDeleteAsync();

    public async Task DeleteByMapIdAsync(Guid mapId) =>
        await Context.Set<Layer>()
            .Where(layer => layer.MapId == mapId)
            .ExecuteDeleteAsync();

    public async Task<List<Layer>> GetByMapIdAsync(Guid mapId) =>
        await Context.Set<Layer>().Where(layer => layer.MapId == mapId).ToListAsync();

    public override async Task UpdateAsync(Layer entity, CancellationToken ct = default) =>
        await Context.Set<Layer>().Where(layer => layer.Id == entity.Id).ExecuteUpdateAsync(
            calls => calls
                .SetProperty(layer => layer.Name, entity.Name)
                .SetProperty(layer => layer.Description, entity.Description)
                .SetProperty(layer => layer.Index, entity.Index)
                .SetProperty(layer => layer.IsVisible, entity.IsVisible)
                .SetProperty(layer => layer.MapId, entity.MapId)
                .SetProperty(layer => layer.Data, entity.Data)
                .SetProperty(layer => layer.StyleOptions, entity.StyleOptions)
                .SetProperty(layer => layer.Type, entity.Type),
            ct);
}

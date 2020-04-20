using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThematicMapCreator.Api.Contracts;
using ThematicMapCreator.Api.Models;

namespace ThematicMapCreator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController : Controller
    {
        private readonly ThematicMapDbContext context;

        public MapsController(ThematicMapDbContext context)
        {
            this.context = context;
        }

        [HttpDelete("{mapId:guid}")]
        public async Task DeleteMap(Guid mapId)
        {
            var map = await context.Maps.FirstOrDefaultAsync(m => m.Id == mapId);
            if (map == null)
            {
                throw new KeyNotFoundException();
            }

            var layers = await context.Layers.Where(layer => layer.MapId == mapId).ToListAsync();
            context.Layers.RemoveRange(layers);

            context.Maps.Remove(map);

            await context.SaveChangesAsync();
        }

        [HttpGet("{mapId:guid}/layers")]
        public async Task<IEnumerable<LayerOverview>> GetLayersByMapId(Guid mapId)
        {
            return await context.Layers.AsNoTracking()
                .Where(layer => layer.MapId == mapId)
                .ProjectToType<LayerOverview>()
                .ToListAsync();
        }

        [HttpGet("{mapId:guid}")]
        public async Task<MapOverview> GetMap(Guid mapId)
        {
            var result = await context.Maps.AsNoTracking()
                .FirstOrDefaultAsync(map => map.Id == mapId);
            return result.Adapt<MapOverview>();
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IEnumerable<MapOverview>> GetMapsByUserId(Guid userId)
        {
            return await context.Maps.AsNoTracking()
                .Where(map => map.UserId == userId)
                .ProjectToType<MapOverview>()
                .ToListAsync();
        }

        [HttpPut]
        public async Task<Guid> SaveMap([FromBody] SaveMapRequest request)
        {
            Map existingMap = request.Id != Guid.Empty
                ? await context.Maps.AsNoTracking().FirstOrDefaultAsync(map => map.Id == request.Id)
                : null;

            Guid mapId;
            if (existingMap == null || existingMap.UserId != request.UserId)
            {
                mapId = await AddMap(request);
            }
            else
            {
                mapId = await UpdateMap(request);
            }

            await context.SaveChangesAsync();

            return mapId;
        }

        private async Task<Guid> AddMap(SaveMapRequest request)
        {
            var map = request.Adapt<Map>();
            map.Id = Guid.NewGuid();

            var layers = request.Layers.Adapt<List<Layer>>();
            foreach (var layer in layers)
            {
                layer.Id = Guid.NewGuid();
                layer.MapId = map.Id;
            }

            await context.Maps.AddAsync(map);
            await context.Layers.AddRangeAsync(layers);

            return map.Id;
        }

        private async Task<Guid> UpdateMap(SaveMapRequest request)
        {
            var map = request.Adapt<Map>();

            var layers = request.Layers.Adapt<List<Layer>>();
            foreach (var layer in layers)
            {
                layer.MapId = map.Id;
            }

            var oldLayers = await context.Layers.AsNoTracking().Where(layer => layer.MapId == map.Id).ToListAsync();

            var newLayers = layers.Where(layer => oldLayers.All(old => old.Id != layer.Id));
            foreach (var layer in newLayers)
            {
                layer.Id = Guid.NewGuid();
            }

            var updatedLayers = layers.Where(layer => oldLayers.Any(old => old.Id == layer.Id));
            var deletedLayers = oldLayers.Where(old => layers.All(layer => old.Id != layer.Id));

            context.Maps.Update(map);
            await context.Layers.AddRangeAsync(newLayers);
            context.Layers.UpdateRange(updatedLayers);
            context.Layers.RemoveRange(deletedLayers);

            return map.Id;
        }
    }
}

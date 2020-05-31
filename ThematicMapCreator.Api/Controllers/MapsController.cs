using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThematicMapCreator.Api.Contracts;
using ThematicMapCreator.Api.Core;
using ThematicMapCreator.Api.Models;
using Z.EntityFramework.Plus;

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
            await context.Layers.Where(layer => layer.MapId == mapId).DeleteAsync();
            await context.Maps.Where(m => m.Id == mapId).DeleteAsync();
            await context.SaveChangesAsync();
        }

        [HttpGet("{mapId:guid}/layers")]
        public async Task<IEnumerable<LayerOverview>> GetLayersByMapId(Guid mapId)
        {
            return await context.Layers
                .Where(layer => layer.MapId == mapId)
                .ProjectToType<LayerOverview>()
                .ToListAsync();
        }

        [HttpGet("{mapId:guid}")]
        public async Task<MapOverview> GetMap(Guid mapId)
        {
            return await context.Maps
                .FirstOrDefaultAsync(map => map.Id == mapId)
                .AdaptAsync<Map, MapOverview>();
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IEnumerable<MapOverview>> GetMapsByUserId(Guid userId)
        {
            return await context.Maps
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
            var layers = request.Layers.Adapt<List<Layer>>();
            foreach (var layer in layers)
            {
                layer.MapId = request.Id;
            }

            var newLayers = layers.Where(layer => layer.Id == Guid.Empty);
            foreach (var layer in newLayers)
            {
                layer.Id = Guid.NewGuid();
            }

            var oldLayers = layers.Except(newLayers);

            await context.Maps.Where(map => map.Id == request.Id).UpdateAsync(map => new Map
            {
                Name = request.Name,
                Description = request.Description
            });
            await context.Layers.AddRangeAsync(newLayers);
            await context.Layers.Where(old => !oldLayers.Select(layer => layer.Id).Contains(old.Id)).DeleteAsync();
            foreach (var layer in oldLayers)
            {
                await context.Layers.Where(old => old.Id == layer.Id).UpdateAsync(old => new Layer
                {
                    Visible = layer.Visible,
                    Index = layer.Index,
                    Name = layer.Name,
                    StyleOptions = layer.StyleOptions
                });
            }

            return request.Id;
        }
    }
}

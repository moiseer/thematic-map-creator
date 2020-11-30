using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Services;
using ThematicMapCreator.Host.Extensions;

namespace ThematicMapCreator.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController : Controller
    {
        private readonly IMapsService service;

        public MapsController(IMapsService service)
        {
            this.service = service;
        }

        [HttpDelete("{id:guid}")]
        public async Task DeleteMap(Guid id)
        {
            await service.DeleteAsync(id);
        }

        [HttpGet("{id:guid}/layers")]
        public async Task<IEnumerable<LayerDto>> GetLayersByMapId(Guid id)
        {
            return (await service.GetLayersAsync(id)).ConvertAll(layer => layer.ToDto());
        }

        [HttpGet("{id:guid}")]
        public async Task<MapDto> GetMap(Guid id)
        {
            return (await service.GetDetailsAsync(id)).ToDto();
        }

        [HttpPut]
        public async Task<Guid> SaveMap([FromBody] SaveMapRequest request)
        {
            return await service.SaveAsync(request);
        }
    }
}

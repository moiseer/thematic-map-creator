using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Commands;
using ThematicMapCreator.Domain.Queries;
using ThematicMapCreator.Host.Extensions;

namespace ThematicMapCreator.Host.Controllers;

[ApiController]
[Route("api/maps")]
public sealed class MapsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MapsController(IMediator mediator) => _mediator = mediator;

    [HttpDelete("{mapId:guid}")]
    public async Task DeleteMapAsync(Guid mapId, CancellationToken ct) =>
        await _mediator.Send(new MapDeleteCommand(mapId), ct);

    [HttpGet("{mapId:guid}/layers")]
    public async Task<IEnumerable<LayerDto>> GetLayersByMapIdAsync(Guid mapId, CancellationToken ct)
    {
        var layers = await _mediator.Send(new LayersQuery(mapId), ct);
        return layers.Select(layer => layer.ToDto());
    }

    [HttpGet("{mapId:guid}")]
    public async Task<MapDto> GetMap(Guid mapId, CancellationToken ct)
    {
        var map = await _mediator.Send(new MapDetailsQuery(mapId), ct);
        return map.ToDto();
    }

    [HttpPut]
    public async Task SaveMap([FromBody] SaveMapRequest request, CancellationToken ct) =>
        await _mediator.Send(request.ToCommand(), ct);
}

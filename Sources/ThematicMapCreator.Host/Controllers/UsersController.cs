using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Queries;
using ThematicMapCreator.Host.Extensions;

namespace ThematicMapCreator.Host.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator mediator;

    public UsersController(IMediator mediator) => this.mediator = mediator;

    [HttpGet("{userId:guid}/maps")]
    public async Task<IEnumerable<MapDto>> GetMapsAsync(Guid userId, CancellationToken ct)
    {
        var maps = await mediator.Send(new MapsQuery(userId), ct);
        return maps.Select(map => map.ToDto());
    }
}

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
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{userId:guid}/maps")]
    public async Task<IEnumerable<MapDto>> GetMapsAsync(Guid userId, CancellationToken ct)
    {
        var maps = await _mediator.Send(new MapsQuery(userId), ct);
        return maps.Select(map => map.ToDto());
    }
}

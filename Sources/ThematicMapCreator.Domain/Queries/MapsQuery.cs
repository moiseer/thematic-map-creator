using System;
using System.Collections.Generic;
using MediatR;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Queries;

public sealed record MapsQuery(Guid UserId) : IRequest<IEnumerable<Map>>;

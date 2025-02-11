using MediatR;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Queries;

public sealed record MapDetailsQuery(Guid MapId) : IRequest<Map>;

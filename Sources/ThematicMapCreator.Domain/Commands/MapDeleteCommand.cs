using System;
using MediatR;

namespace ThematicMapCreator.Domain.Commands;

public sealed record MapDeleteCommand(Guid MapId) : IRequest;

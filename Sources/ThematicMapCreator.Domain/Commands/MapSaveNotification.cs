using MediatR;

namespace ThematicMapCreator.Domain.Commands;

public sealed record MapSaveNotification(Guid Id) : INotification;

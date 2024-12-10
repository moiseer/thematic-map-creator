using System;
using MediatR;

namespace ThematicMapCreator.Domain.Commands;

public sealed record MapDeleteNotification(Guid Id) : INotification;

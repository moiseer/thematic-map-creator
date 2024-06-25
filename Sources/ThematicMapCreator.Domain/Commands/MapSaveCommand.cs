using System;
using System.Collections.Generic;
using MediatR;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Commands;

public record MapSaveCommand : IRequest
{
    public string? Description { get; set; }
    public Guid? Id { get; set; }
    public List<Layer> Layers { get; set; } = new();
    public string? Name { get; set; }
    public Guid UserId { get; set; }

    public sealed record Layer
    {
        public string? Data { get; set; }
        public string? Description { get; set; }
        public Guid? Id { get; set; }
        public int Index { get; set; }
        public bool IsVisible { get; set; }
        public string? Name { get; set; }
        public string? StyleOptions { get; set; }
        public LayerType Type { get; set; }
    }
}

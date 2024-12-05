using System;
using System.Collections.Generic;

namespace ThematicMapCreator.Contracts;

public sealed record SaveMapRequest
{
    public string? Description { get; set; }
    public Guid? Id { get; set; }
    public List<SaveLayerRequest> Layers { get; set; } = new();
    public string? Name { get; set; }
    public Guid UserId { get; set; }
}

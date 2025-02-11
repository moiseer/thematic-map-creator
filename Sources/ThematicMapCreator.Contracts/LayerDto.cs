﻿namespace ThematicMapCreator.Contracts;

public sealed record LayerDto
{
    public string? Data { get; set; }
    public string? Description { get; set; }
    public Guid Id { get; set; }
    public int Index { get; set; }
    public bool IsVisible { get; set; }
    public Guid MapId { get; set; }
    public string? Name { get; set; }
    public string? StyleOptions { get; set; }
    public int Type { get; set; }
}

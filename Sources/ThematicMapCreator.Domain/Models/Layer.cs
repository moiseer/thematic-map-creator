using Core.Dal.Models;

namespace ThematicMapCreator.Domain.Models;

public sealed class Layer : IEntity<Guid>
{
    public string Data { get; set; } = null!;
    public string? Description { get; set; }
    public Guid Id { get; set; }
    public int Index { get; set; }
    public bool IsVisible { get; set; }
    public Map Map { get; set; } = null!;
    public Guid MapId { get; set; }
    public string Name { get; set; } = null!;
    public string StyleOptions { get; set; } = null!;
    public LayerType Type { get; set; }
}

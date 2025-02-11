using Core.Dal.Models;

namespace ThematicMapCreator.Domain.Models;

public sealed class User : IEntity<Guid>
{
    public string Email { get; set; } = null!;
    public Guid Id { get; set; }
    public List<Map> Maps { get; set; } = new();
    public string Name { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}

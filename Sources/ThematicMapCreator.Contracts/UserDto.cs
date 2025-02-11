namespace ThematicMapCreator.Contracts;

public sealed record UserDto
{
    public string? Email { get; set; }
    public Guid Id { get; set; }
    public string? Name { get; set; }
}

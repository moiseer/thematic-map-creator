using System;

namespace ThematicMapCreator.Contracts
{
    public class MapDto
    {
        public string? Description { get; set; }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid UserId { get; set; }
    }
}

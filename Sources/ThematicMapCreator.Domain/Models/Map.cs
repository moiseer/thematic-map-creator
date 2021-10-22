using System;
using System.Collections.Generic;
using Core.Dal.Models;

namespace ThematicMapCreator.Domain.Models
{
    public class Map : IEntity<Guid>
    {
        public string? Description { get; set; }
        public Guid Id { get; set; }
        public List<Layer> Layers { get; set; } = new();
        public string Name { get; set; } = null!;
        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}

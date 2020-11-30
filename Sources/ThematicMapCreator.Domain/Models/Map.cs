using System;
using System.Collections.Generic;
using Core.Dal.Models;

namespace ThematicMapCreator.Domain.Models
{
    public class Map : IEntity<Guid>
    {
        public string Description { get; set; }
        public Guid Id { get; set; }
        public List<Layer> Layers { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}

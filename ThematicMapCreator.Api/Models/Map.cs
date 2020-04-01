using System;
using System.Collections.Generic;

namespace ThematicMapCreator.Api.Models
{
    public class Map
    {
        public Guid Id { get; set; }
        public List<Layer> Layers { get; set; }
        public string Name { get; set; }
        public string Settings { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}

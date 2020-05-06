using System;

namespace ThematicMapCreator.Api.Models
{
    public class LayerOptions
    {
        public Guid LayerId { get; set; }
        public Layer Layer { get; set; }
        public LayerType Type { get; set; }
    }
}

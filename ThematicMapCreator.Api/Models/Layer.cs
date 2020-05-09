using System;

namespace ThematicMapCreator.Api.Models
{
    public class Layer
    {
        public string Data { get; set; }
        public Guid Id { get; set; }
        public int Index { get; set; }
        public Map Map { get; set; }
        public Guid MapId { get; set; }
        public string Name { get; set; }
        public LayerStyle Style { get; set; }
        public LayerType Type { get; set; }
        public bool Visible { get; set; }
    }
}

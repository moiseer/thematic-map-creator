using System;
using Core.Dal.Models;

namespace ThematicMapCreator.Domain.Models
{
    public class Layer : IEntity<Guid>
    {
        public string Data { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
        public int Index { get; set; }
        public bool IsVisible { get; set; }
        public Map Map { get; set; }
        public Guid MapId { get; set; }
        public string Name { get; set; }
        public string StyleOptions { get; set; }
        public LayerType Type { get; set; }
    }
}

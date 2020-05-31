using System;
using ThematicMapCreator.Api.Contracts.LayerStyleOptions;
using ThematicMapCreator.Api.Models;

namespace ThematicMapCreator.Api.Contracts
{
    public class LayerOverview
    {
        public string Data { get; set; }
        public Guid Id { get; set; }
        public int Index { get; set; }
        public Guid MapId { get; set; }
        public string Name { get; set; }
        public ILayerStyleOptions StyleOptions { get; set; }
        public LayerType Type { get; set; }
        public bool Visible { get; set; }
    }
}

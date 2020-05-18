using System;

namespace ThematicMapCreator.Api.Contracts.LayerStyleOptions
{
    public class SimpleStyleOptions : ILayerStyleOptions
    {
        public string Color { get; set; }
        public string FillColor { get; set; }
        public int Size { get; set; }
        public LayerStyle Style { get; } = LayerStyle.None;
    }
}

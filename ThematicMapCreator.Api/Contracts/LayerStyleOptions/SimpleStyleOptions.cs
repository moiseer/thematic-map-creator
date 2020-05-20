using System;

namespace ThematicMapCreator.Api.Contracts.LayerStyleOptions
{
    public class SimpleStyleOptions : ILayerStyleOptions
    {
        public string Color { get; set; }
        public string FillColor { get; set; }
        public double Size { get; set; }
        public LayerStyle Style { get; } = LayerStyle.None;
    }
}

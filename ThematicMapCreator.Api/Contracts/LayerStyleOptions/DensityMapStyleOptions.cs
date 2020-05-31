using System;

namespace ThematicMapCreator.Api.Contracts.LayerStyleOptions
{
    public class DensityMapStyleOptions : ILayerStyleOptions
    {
        public string Color { get; set; }
        public string FillColor { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public string PropertyName { get; set; }
        public double Size { get; set; }
        public LayerStyle Style { get; } = LayerStyle.DensityMap;
    }
}

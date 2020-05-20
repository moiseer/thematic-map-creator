using System;

namespace ThematicMapCreator.Api.Contracts.LayerStyleOptions
{
    public class GraduatedColorsStyleOptions : ILayerStyleOptions
    {
        public string MaxColor { get; set; }
        public double MaxValue { get; set; }
        public string MinColor { get; set; }
        public double MinValue { get; set; }
        public string PropertyName { get; set; }
        public double Size { get; set; }
        public LayerStyle Style { get; } = LayerStyle.GraduatedColors;
    }
}

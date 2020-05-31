using System;

namespace ThematicMapCreator.Api.Contracts.LayerStyleOptions
{
    public class GraduatedCharactersStyleOption : ILayerStyleOptions
    {
        public string Color { get; set; }
        public string FillColor { get; set; }
        public double MaxSize { get; set; }
        public double MaxValue { get; set; }
        public double MinSize { get; set; }
        public double MinValue { get; set; }
        public string PropertyName { get; set; }
        public LayerStyle Style { get; } = LayerStyle.GraduatedCharacters;
    }
}

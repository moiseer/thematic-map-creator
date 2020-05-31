using System;
using System.Collections.Generic;

namespace ThematicMapCreator.Api.Contracts.LayerStyleOptions
{
    public class ChartDiagramStyleOptions : ILayerStyleOptions
    {
        public string Color { get; set; }
        public string FillColor { get; set; }
        public Dictionary<string, string> PropertyNameColors { get; set; }
        public double Size { get; set; }
        public LayerStyle Style { get; } = LayerStyle.ChartDiagram;
    }
}

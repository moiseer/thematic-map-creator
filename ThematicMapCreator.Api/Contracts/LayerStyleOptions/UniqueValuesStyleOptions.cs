using System;
using System.Collections.Generic;

namespace ThematicMapCreator.Api.Contracts.LayerStyleOptions
{
    public class UniqueValuesStyleOptions : ILayerStyleOptions
    {
        public string PropertyName { get; set; }
        public LayerStyle Style { get; } = LayerStyle.UniqueValues;
        public Dictionary<string, SimpleStyleOptions> ValueStyleOptions { get; set; }
    }
}

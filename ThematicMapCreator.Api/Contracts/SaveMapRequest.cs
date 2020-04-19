using System;
using System.Collections.Generic;

namespace ThematicMapCreator.Api.Contracts
{
    public class SaveMapRequest
    {
        public string Description { get; set; }
        public Guid Id { get; set; }
        public List<LayerOverview> Layers { get; set; }
        public string Name { get; set; }
        public string Settings { get; set; }
        public Guid UserId { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace ThematicMapCreator.Contracts
{
    public class SaveMapRequest
    {
        public string Description { get; set; }
        public Guid? Id { get; set; }
        public List<SaveLayerRequest> Layers { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }
}

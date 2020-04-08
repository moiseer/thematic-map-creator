using System;

namespace ThematicMapCreator.Api.Contracts
{
    public class LayerOverview
    {
        public string Data { get; set; }
        public Guid Id { get; set; }
        public Guid MapId { get; set; }
        public string Name { get; set; }
        public string Settings { get; set; }
    }
}

using System;

namespace ThematicMapCreator.Api.Contracts
{
    public class MapOverview
    {
        public string Description { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }
}

using System;

namespace ThematicMapCreator.Api.Contracts
{
    public class UserOverview
    {
        public string Email { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

using System;

namespace ThematicMapCreator.Contracts
{
    public class UserDto
    {
        public string Email { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

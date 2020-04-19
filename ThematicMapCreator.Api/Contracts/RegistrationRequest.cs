using System;

namespace ThematicMapCreator.Api.Contracts
{
    public class RegistrationRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}

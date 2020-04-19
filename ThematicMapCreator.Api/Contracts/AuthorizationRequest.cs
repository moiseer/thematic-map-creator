using System;

namespace ThematicMapCreator.Api.Contracts
{
    public class AuthorizationRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

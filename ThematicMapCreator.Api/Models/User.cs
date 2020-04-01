using System;
using System.Collections.Generic;

namespace ThematicMapCreator.Api.Models
{
    public class User
    {
        public string Email { get; set; }
        public Guid Id { get; set; }
        public List<Map> Maps { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Core.Dal.Models;

namespace ThematicMapCreator.Domain.Models
{
    public class User : IEntity<Guid>
    {
        public string Email { get; set; }
        public Guid Id { get; set; }
        public List<Map> Maps { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
    }
}

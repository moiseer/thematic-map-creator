using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Services
{
    public interface IUsersService
    {
        Task<List<Map>> GetMapsAsync(Guid id);
    }
}

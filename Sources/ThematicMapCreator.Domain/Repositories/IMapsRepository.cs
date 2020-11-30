using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dal.Repositories;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Repositories
{
    public interface IMapsRepository : IAsyncCrudRepository<Map, Guid>
    {
        Task<List<Map>> GetByUserIdAsync(Guid userId);
    }
}

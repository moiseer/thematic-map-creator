using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dal.Repositories;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Repositories
{
    public interface ILayersRepository : IAsyncCrudRepository<Layer, Guid>
    {
        Task AddAsync(IEnumerable<Layer> layers);
        Task DeleteByMapIdAsync(Guid mapId, IEnumerable<Guid> excludedLayerIds);
        Task DeleteByMapIdAsync(Guid mapId);
        Task<List<Layer>> GetByMapIdAsync(Guid mapId);
    }
}

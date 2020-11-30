using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Services
{
    public interface IMapsService
    {
        Task DeleteAsync(Guid id);
        Task<Map> GetDetailsAsync(Guid id);
        Task<List<Layer>> GetLayersAsync(Guid id);
        Task<Guid> SaveAsync(SaveMapRequest request);
    }
}

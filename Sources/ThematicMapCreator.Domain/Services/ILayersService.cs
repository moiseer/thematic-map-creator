using System;
using System.Threading.Tasks;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Services
{
    public interface ILayersService
    {
        Task AddAsync(Layer layer);
        Task DeleteAsync(Guid id);
        Task<Layer> GetDetailsAsync(Guid id);
    }
}

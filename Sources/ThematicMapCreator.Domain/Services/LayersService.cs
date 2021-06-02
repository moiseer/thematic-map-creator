using System;
using System.Threading.Tasks;
using Core.Dal;
using Microsoft.Extensions.Logging;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Services
{
    public class LayersService : ILayersService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ILogger logger;

        public LayersService(IUnitOfWorkFactory unitOfWorkFactory, ILogger<LayersService> logger)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.logger = logger;
        }

        public async Task AddAsync(Layer layer)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<ILayersRepository>();
            await repository.AddAsync(layer);
            await unitOfWork.CommitAsync();

            logger.LogDebug("Layer {LayerId} added", layer.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<ILayersRepository>();
            await repository.DeleteAsync(id);
            await unitOfWork.CommitAsync();

            logger.LogDebug("Layer {LayerId} deleted", id);
        }

        public async Task<Layer> GetDetailsAsync(Guid id)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<ILayersRepository>();
            var layer = await repository.GetAsync(id)
                .ThrowOnEmptyAsync(() => new TmcException(TmcError.Layer.NotFound));
            await unitOfWork.CommitAsync();

            return layer;
        }
    }
}

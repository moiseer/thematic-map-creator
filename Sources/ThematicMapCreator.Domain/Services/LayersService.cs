using System;
using System.Threading.Tasks;
using Core.Dal;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Services
{
    public class LayersService : ILayersService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public LayersService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task AddAsync(Layer layer)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<ILayersRepository>();
            await repository.AddAsync(layer);
            await unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<ILayersRepository>();
            await repository.DeleteAsync(id);
            await unitOfWork.CommitAsync();
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

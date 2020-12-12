using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Dal;
using FluentValidation;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Services
{
    public class MapsService : IMapsService
    {
        private readonly IValidator<SaveMapRequest> saveMapValidator;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public MapsService(IUnitOfWorkFactory unitOfWorkFactory, IValidator<SaveMapRequest> saveMapValidator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.saveMapValidator = saveMapValidator;
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            await repository.DeleteAsync(id);
            await unitOfWork.CommitAsync();
        }

        public async Task<Map> GetDetailsAsync(Guid id)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            var map = await repository.GetAsync(id)
                .ThrowOnEmptyAsync(() => new TmcException(TmcError.Map.NotFound));
            await unitOfWork.CommitAsync();

            return map;
        }

        public async Task<List<Layer>> GetLayersAsync(Guid id)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<ILayersRepository>();
            var layers = await repository.GetByMapIdAsync(id);
            await unitOfWork.CommitAsync();

            return layers;
        }

        /// <summary>
        /// Сохранение карты и её слоев.
        /// </summary>
        /// <param name="request">Запрос на сохранение карты.</param>
        /// <returns>Идентификатор сохраненной карты.</returns>
        public async Task<Guid> SaveAsync(SaveMapRequest request)
        {
            await saveMapValidator.ValidateAsync(request).ThrowOnErrorsAsync();

            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var mapsRepository = unitOfWork.GetRepository<IMapsRepository>();

            Map existingMap = request.Id.HasValue
                ? await mapsRepository.GetAsync(request.Id.Value)
                : null;

            Guid mapId;
            if (existingMap == null || existingMap.UserId != request.UserId)
            {
                mapId = await AddMapAsync(request, unitOfWork);
            }
            else
            {
                mapId = await UpdateMapAsync(request, existingMap, unitOfWork);
            }

            await unitOfWork.CommitAsync();

            return mapId;
        }

        private static Layer CreateLayer(SaveLayerRequest request, Guid mapId) => new Layer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            MapId = mapId,
            Data = request.Data,
            Index = request.Index,
            IsVisible = request.IsVisible,
            StyleOptions = request.StyleOptions,
            Type = (LayerType)request.Type
        };

        private static Map CreateMap(SaveMapRequest request) => new Map
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId
        };

        private async Task<Guid> AddMapAsync(SaveMapRequest request, IUnitOfWork unitOfWork)
        {
            var mapsRepository = unitOfWork.GetRepository<IMapsRepository>();
            var layerRepository = unitOfWork.GetRepository<ILayersRepository>();

            Map map = CreateMap(request);
            await mapsRepository.AddAsync(map);

            List<Layer> layers = request.Layers.ConvertAll(layer => CreateLayer(layer, map.Id));
            await layerRepository.AddAsync(layers);

            return map.Id;
        }

        /// <remarks>Зависит от change detection.</remarks>
        private async Task<Guid> UpdateMapAsync(SaveMapRequest request, Map map, IUnitOfWork unitOfWork)
        {
            var layerRepository = unitOfWork.GetRepository<ILayersRepository>();

            map.Name = request.Name;
            map.Description = request.Description;

            IEnumerable<Layer> newLayers = request.Layers
                .Where(layer => !layer.Id.HasValue)
                .Select(layer => CreateLayer(layer, map.Id));

            HashSet<Guid> updateLayerIds = request.Layers
                .Where(layer => layer.Id.HasValue)
                .Select(layer => layer.Id.Value)
                .ToHashSet();

            await layerRepository.DeleteByMapIdAsync(map.Id, excludedLayerIds: updateLayerIds);
            await layerRepository.AddAsync(newLayers);
            List<Layer> updateLayers = await layerRepository.GetAsync(updateLayerIds);
            foreach (var layer in updateLayers)
            {
                var update = request.Layers.Single(l => l.Id.HasValue && l.Id.Value == layer.Id);
                layer.Name = update.Name;
                layer.Description = update.Description;
                layer.IsVisible = update.IsVisible;
                layer.Index = update.Index;
                layer.StyleOptions = update.StyleOptions;
            }

            return map.Id;
        }
    }
}

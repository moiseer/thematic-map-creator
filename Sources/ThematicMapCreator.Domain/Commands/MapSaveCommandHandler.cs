using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ThematicMapCreator.Domain.Extensions;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Commands;

public sealed class MapSaveCommandHandler : IRequestHandler<MapSaveCommand>
{
    private readonly ILogger<MapSaveCommandHandler> logger;
    private readonly IValidator<MapSaveCommand> saveMapValidator;
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    public MapSaveCommandHandler(
        ILogger<MapSaveCommandHandler> logger,
        IValidator<MapSaveCommand> saveMapValidator,
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        this.logger = logger;
        this.saveMapValidator = saveMapValidator;
        this.unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(MapSaveCommand request, CancellationToken cancellationToken)
    {
        await saveMapValidator.ValidateAsync(request).ThrowOnErrorsAsync();

        await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
        var mapsRepository = unitOfWork.GetRepository<IMapsRepository>();

        Map? existingMap = request.Id.HasValue
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

        logger.LogInformation("Map {MapId} saved", mapId);
    }

    private static Layer CreateLayer(MapSaveCommand.Layer request, Guid mapId) => new()
    {
        Id = Guid.NewGuid(),
        Name = request.Name!,
        Description = request.Description,
        MapId = mapId,
        Data = request.Data!,
        Index = request.Index,
        IsVisible = request.IsVisible,
        StyleOptions = request.StyleOptions!,
        Type = request.Type
    };

    private static Map CreateMap(MapSaveCommand request) => new()
    {
        Id = Guid.NewGuid(),
        Name = request.Name!,
        Description = request.Description,
        UserId = request.UserId
    };

    private async Task<Guid> AddMapAsync(MapSaveCommand request, IUnitOfWork unitOfWork)
    {
        var mapsRepository = unitOfWork.GetRepository<IMapsRepository>();
        var layerRepository = unitOfWork.GetRepository<ILayersRepository>();

        Map map = CreateMap(request);
        await mapsRepository.AddAsync(map);

        List<Layer> layers = request.Layers.ConvertAll(layer => CreateLayer(layer, map.Id));
        await layerRepository.AddAsync(layers);

        logger.LogDebug("Map {MapId} added", map.Id);
        return map.Id;
    }

    private async Task<Guid> UpdateMapAsync(MapSaveCommand request, Map map, IUnitOfWork unitOfWork)
    {
        var layerRepository = unitOfWork.GetRepository<ILayersRepository>();

        map.Name = request.Name!;
        map.Description = request.Description;

        IEnumerable<Layer> newLayers = request.Layers
            .Where(layer => !layer.Id.HasValue)
            .Select(layer => CreateLayer(layer, map.Id));

        HashSet<Guid> updateLayerIds = request.Layers
            .Where(layer => layer.Id.HasValue)
            .Select(layer => layer.Id!.Value)
            .ToHashSet();

        await layerRepository.DeleteByMapIdAsync(map.Id, excludedLayerIds: updateLayerIds);
        await layerRepository.AddAsync(newLayers);
        List<Layer> updateLayers = await layerRepository.GetAsync(updateLayerIds);
        foreach (var layer in updateLayers)
        {
            var update = request.Layers.Single(l => l.Id.HasValue && l.Id.Value == layer.Id);
            layer.Name = update.Name!;
            layer.Description = update.Description;
            layer.IsVisible = update.IsVisible;
            layer.Index = update.Index;
            layer.StyleOptions = update.StyleOptions!;

            await layerRepository.UpdateAsync(layer);
        }

        logger.LogDebug("Map {MapId} updated", map.Id);
        return map.Id;
    }
}
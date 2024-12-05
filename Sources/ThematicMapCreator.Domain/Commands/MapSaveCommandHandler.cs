using System;
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
    private readonly ILogger<MapSaveCommandHandler> _logger;
    private readonly IValidator<MapSaveCommand> _saveMapValidator;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public MapSaveCommandHandler(
        ILogger<MapSaveCommandHandler> logger,
        IValidator<MapSaveCommand> saveMapValidator,
        IUnitOfWorkFactory unitOfWorkFactory)
    {
        _logger = logger;
        _saveMapValidator = saveMapValidator;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(MapSaveCommand request, CancellationToken cancellationToken)
    {
        await _saveMapValidator.ValidateAsync(request, cancellationToken).ThrowOnErrorsAsync();

        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync(cancellationToken);
        var mapsRepository = unitOfWork.GetRepository<IMapsRepository>();

        var existingMap = request.Id.HasValue
            ? await mapsRepository.GetAsync(request.Id.Value, cancellationToken)
            : null;

        Guid mapId;
        if (existingMap == null || existingMap.UserId != request.UserId)
        {
            mapId = await AddMapAsync(request, unitOfWork, cancellationToken);
        }
        else
        {
            mapId = await UpdateMapAsync(request, existingMap, unitOfWork, cancellationToken);
        }

        await unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Map {MapId} saved", mapId);
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
        Type = request.Type,
    };

    private static Map CreateMap(MapSaveCommand request) => new()
    {
        Id = Guid.NewGuid(),
        Name = request.Name!,
        Description = request.Description,
        UserId = request.UserId,
    };

    private async Task<Guid> AddMapAsync(MapSaveCommand request, IUnitOfWork unitOfWork, CancellationToken ct)
    {
        var mapsRepository = unitOfWork.GetRepository<IMapsRepository>();
        var layerRepository = unitOfWork.GetRepository<ILayersRepository>();

        var map = CreateMap(request);
        await mapsRepository.AddAsync(map, ct);

        var layers = request.Layers.ConvertAll(layer => CreateLayer(layer, map.Id));
        await layerRepository.AddAsync(layers);

        _logger.LogDebug("Map {MapId} added", map.Id);
        return map.Id;
    }

    private async Task<Guid> UpdateMapAsync(MapSaveCommand request, Map map, IUnitOfWork unitOfWork, CancellationToken ct)
    {
        var layerRepository = unitOfWork.GetRepository<ILayersRepository>();

        map.Name = request.Name!;
        map.Description = request.Description;

        var newLayers = request.Layers
            .Where(layer => !layer.Id.HasValue)
            .Select(layer => CreateLayer(layer, map.Id));

        var updateLayerIds = request.Layers
            .Where(layer => layer.Id.HasValue)
            .Select(layer => layer.Id!.Value)
            .ToHashSet();

        await layerRepository.DeleteByMapIdAsync(map.Id, updateLayerIds);
        await layerRepository.AddAsync(newLayers);
        var updateLayers = await layerRepository.GetAsync(updateLayerIds, ct);
        foreach (var layer in updateLayers)
        {
            var update = request.Layers.Single(l => l.Id.HasValue && l.Id.Value == layer.Id);
            layer.Name = update.Name!;
            layer.Description = update.Description;
            layer.IsVisible = update.IsVisible;
            layer.Index = update.Index;
            layer.StyleOptions = update.StyleOptions!;

            await layerRepository.UpdateAsync(layer, ct);
        }

        _logger.LogDebug("Map {MapId} updated", map.Id);
        return map.Id;
    }
}

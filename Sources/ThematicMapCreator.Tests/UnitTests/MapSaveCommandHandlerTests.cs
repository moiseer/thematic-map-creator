using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ThematicMapCreator.Domain.Commands;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.UnitTests;

public sealed class MapSaveCommandHandlerTests
{
    private readonly MapSaveCommandHandler _handler;
    private readonly Mock<ILayersRepository> _layersRepositoryMock = new();
    private readonly Mock<IMapsRepository> _mapsRepositoryMock = new();
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock = new();
    private readonly Mock<IValidator<MapSaveCommand>> _validatorMock = new();

    public MapSaveCommandHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<IMapsRepository>()).Returns(_mapsRepositoryMock.Object);
        unitOfWorkMock.Setup(uow => uow.GetRepository<ILayersRepository>()).Returns(_layersRepositoryMock.Object);

        _unitOfWorkFactoryMock
            .Setup(factory => factory.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unitOfWorkMock.Object);

        _validatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<MapSaveCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _handler = new MapSaveCommandHandler(Mock.Of<ILogger<MapSaveCommandHandler>>(), Mock.Of<IPublisher>(), _validatorMock.Object, _unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_MapAndLayersCreated_Success()
    {
        var request = CreateMapSaveCommand();
        request.Layers = Enumerable.Range(0, 3).Select(i => CreateMapSaveCommandLayer(i, LayerType.Point, isVisible: true)).ToList();

        Guid? mapId = null;
        _mapsRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Map>(), It.IsAny<CancellationToken>()))
            .Callback((Map map, CancellationToken _) => mapId = map.Id);

        await _handler.Handle(request, CancellationToken.None);

        _mapsRepositoryMock.Verify(repository => repository.AddAsync(It.Is<Map>(map =>
                    map.Name == request.Name &&
                    map.Description == request.Description &&
                    map.UserId == request.UserId),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _layersRepositoryMock.Verify(repository => repository.AddAsync(It.Is<IEnumerable<Layer>>(layers =>
                request.Layers.TrueForAll(requestLayer =>
                    layers.Any(layer =>
                        layer.Name == requestLayer.Name &&
                        layer.Description == requestLayer.Description &&
                        layer.Data == requestLayer.Data &&
                        layer.StyleOptions == requestLayer.StyleOptions &&
                        layer.Index == requestLayer.Index &&
                        layer.Type == requestLayer.Type &&
                        mapId.HasValue && layer.MapId == mapId.Value)))),
            Times.Once);
    }

    [Fact]
    public async Task Save_MapAndLayersUpdated()
    {
        var existingMap = CreateMap();
        var layersToUpdate = Enumerable.Range(2, 2).Select(i => CreateLayer(existingMap, i)).ToList();

        _mapsRepositoryMock
            .Setup(repository => repository.GetAsync(existingMap.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMap);

        _layersRepositoryMock
            .Setup(repository => repository.GetAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(layersToUpdate);

        var request = CreateMapSaveCommand();
        request.Id = existingMap.Id;
        request.UserId = existingMap.UserId;
        request.Layers = Enumerable.Range(0, 2).Select(i => CreateMapSaveCommandLayer(i, LayerType.Point, isVisible: true)).ToList();
        request.Layers.AddRange(layersToUpdate.Select(layer =>
        {
            var layerRequest = CreateMapSaveCommandLayer(layer.Index, layer.Type, isVisible: true);
            layerRequest.Id = layer.Id;
            return layerRequest;
        }));

        await _handler.Handle(request, CancellationToken.None);

        Assert.Equal(request.Name, existingMap.Name);
        Assert.Equal(request.Description, existingMap.Description);

        Assert.All(request.Layers.Where(requestLayer => requestLayer.Id.HasValue), layerRequest =>
            Assert.Contains(layersToUpdate, layer =>
                layer.Id == layerRequest.Id &&
                layer.Name == layerRequest.Name &&
                layer.Description == layerRequest.Description &&
                layer.Index == layerRequest.Index &&
                layer.IsVisible == layerRequest.IsVisible &&
                layer.StyleOptions == layerRequest.StyleOptions));

        _layersRepositoryMock.Verify(repository => repository.DeleteByMapIdAsync(existingMap.Id,
                It.Is<IEnumerable<Guid>>(layerIds =>
                    layersToUpdate.TrueForAll(layer => layerIds.Any(layerId => layer.Id == layerId)))),
            Times.Once);

        _layersRepositoryMock.Verify(repository => repository.AddAsync(It.Is<IEnumerable<Layer>>(layers =>
                request.Layers.Where(requestLayer => !requestLayer.Id.HasValue).All(requestLayer =>
                    layers.Any(layer =>
                        layer.Name == requestLayer.Name &&
                        layer.Description == requestLayer.Description &&
                        layer.Data == requestLayer.Data &&
                        layer.StyleOptions == requestLayer.StyleOptions &&
                        layer.Index == requestLayer.Index &&
                        layer.Type == requestLayer.Type &&
                        layer.MapId == existingMap.Id)))),
            Times.Once);
    }

    private static Layer CreateLayer(Map map, int index) => new()
    {
        Id = Guid.NewGuid(),
        Name = Guid.NewGuid().ToString(),
        Description = Guid.NewGuid().ToString(),
        MapId = map.Id,
        Map = map,
        Data = Guid.NewGuid().ToString(),
        StyleOptions = Guid.NewGuid().ToString(),
        Index = index,
    };

    private static Map CreateMap() => new()
    {
        Id = Guid.NewGuid(),
        Name = Guid.NewGuid().ToString(),
        Description = Guid.NewGuid().ToString(),
        UserId = Guid.NewGuid(),
    };

    private static MapSaveCommand CreateMapSaveCommand() => new()
    {
        Name = Guid.NewGuid().ToString(),
        Description = Guid.NewGuid().ToString(),
        UserId = Guid.NewGuid(),
    };

    private static MapSaveCommand.Layer CreateMapSaveCommandLayer(int index, LayerType type = LayerType.None, bool isVisible = false) => new()
    {
        Name = Guid.NewGuid().ToString(),
        Description = Guid.NewGuid().ToString(),
        Data = Guid.NewGuid().ToString(),
        StyleOptions = Guid.NewGuid().ToString(),
        Index = index,
        IsVisible = isVisible,
        Type = type,
    };
}

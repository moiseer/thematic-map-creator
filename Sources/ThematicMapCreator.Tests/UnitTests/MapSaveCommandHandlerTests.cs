﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using ThematicMapCreator.Domain.Commands;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.UnitTests;

public sealed class MapSaveCommandHandlerTests
{
    private readonly MapSaveCommandHandler handler;
    private readonly Mock<ILayersRepository> layersRepositoryMock = new();
    private readonly Mock<IMapsRepository> mapsRepositoryMock = new();
    private readonly Mock<IUnitOfWorkFactory> unitOfWorkFactoryMock = new();
    private readonly Mock<IValidator<MapSaveCommand>> validatorMock = new();

    public MapSaveCommandHandlerTests()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(uow => uow.GetRepository<IMapsRepository>()).Returns(mapsRepositoryMock.Object);
        unitOfWorkMock.Setup(uow => uow.GetRepository<ILayersRepository>()).Returns(layersRepositoryMock.Object);

        unitOfWorkFactoryMock
            .Setup(factory => factory.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(unitOfWorkMock.Object);

        validatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<MapSaveCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        handler = new MapSaveCommandHandler(Mock.Of<ILogger<MapSaveCommandHandler>>(), validatorMock.Object, unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_MapAndLayersCreated_Success()
    {
        var request = CreateMapSaveCommand();
        request.Layers = Enumerable.Range(0, 3).Select(i => CreateMapSaveCommandLayer(i, LayerType.Point, isVisible: true)).ToList();

        Guid? mapId = null;
        mapsRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Map>(), It.IsAny<CancellationToken>()))
            .Callback((Map map, CancellationToken _) => mapId = map.Id);

        await handler.Handle(request, CancellationToken.None);

        mapsRepositoryMock.Verify(repository => repository.AddAsync(It.Is<Map>(map =>
                    map.Name == request.Name &&
                    map.Description == request.Description &&
                    map.UserId == request.UserId),
                It.IsAny<CancellationToken>()),
            Times.Once);

        layersRepositoryMock.Verify(repository => repository.AddAsync(It.Is<IEnumerable<Layer>>(layers =>
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

        mapsRepositoryMock
            .Setup(repository => repository.GetAsync(existingMap.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMap);

        layersRepositoryMock
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

        await handler.Handle(request, CancellationToken.None);

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

        layersRepositoryMock.Verify(repository => repository.DeleteByMapIdAsync(existingMap.Id,
                It.Is<IEnumerable<Guid>>(layerIds =>
                    layersToUpdate.TrueForAll(layer => layerIds.Any(layerId => layer.Id == layerId)))),
            Times.Once);

        layersRepositoryMock.Verify(repository => repository.AddAsync(It.Is<IEnumerable<Layer>>(layers =>
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

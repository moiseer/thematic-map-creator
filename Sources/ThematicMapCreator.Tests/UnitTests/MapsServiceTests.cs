using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using ThematicMapCreator.Domain.Services;
using Xunit;

namespace ThematicMapCreator.Tests.UnitTests
{
    public class MapsServiceTests
    {
        private readonly Mock<ILayersRepository> layersRepositoryMock = new Mock<ILayersRepository>();
        private readonly Mock<IMapsRepository> mapsRepositoryMock = new Mock<IMapsRepository>();
        private readonly IMapsService service;
        private readonly Mock<IUnitOfWorkFactory> unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
        private readonly Mock<IValidator<SaveMapRequest>> saveMapRequestValidatorMock = new Mock<IValidator<SaveMapRequest>>();

        public MapsServiceTests()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.GetRepository<IMapsRepository>()).Returns(mapsRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.GetRepository<ILayersRepository>()).Returns(layersRepositoryMock.Object);

            unitOfWorkFactoryMock
                .Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(unitOfWorkMock.Object);

            saveMapRequestValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<SaveMapRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            service = new MapsService(unitOfWorkFactoryMock.Object, saveMapRequestValidatorMock.Object);
        }

        [Fact]
        public async Task Save_MapAndLayersCreated()
        {
            var request = CreateSaveMapRequest();
            request.Layers = Enumerable.Range(0, 3).Select(i => CreateSaveLayerRequest(i, LayerType.Point, isVisible: true)).ToList();

            Guid? mapId = null;
            mapsRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Map>(), It.IsAny<CancellationToken>()))
                .Callback((Map map, CancellationToken _) => mapId = map.Id);

            await service.SaveAsync(request);

            mapsRepositoryMock.Verify(x => x.AddAsync(It.Is<Map>(map =>
                        map.Name == request.Name &&
                        map.Description == request.Description &&
                        map.UserId == request.UserId),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            layersRepositoryMock.Verify(x => x.AddAsync(It.Is<IEnumerable<Layer>>(layers =>
                    layers.Count() == request.Layers.Count &&
                    request.Layers.All(requestLayer =>
                        layers.Any(layer =>
                            layer.Name == requestLayer.Name &&
                            layer.Description == requestLayer.Description &&
                            layer.Data == requestLayer.Data &&
                            layer.StyleOptions == requestLayer.StyleOptions &&
                            layer.Index == requestLayer.Index &&
                            layer.Type == (LayerType)requestLayer.Type &&
                            layer.MapId == mapId.Value)))),
                Times.Once);
        }

        [Fact]
        public async Task Save_MapAndLayersUpdated()
        {
            var existingMap = CreateMap();
            List<Layer> layersToUpdate = Enumerable.Range(2, 2).Select(i => CreateLayer(existingMap, i)).ToList();

            mapsRepositoryMock
                .Setup(x => x.GetAsync(existingMap.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMap);

            layersRepositoryMock
                .Setup(x => x.GetAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(layersToUpdate);

            var request = CreateSaveMapRequest();
            request.Id = existingMap.Id;
            request.UserId = existingMap.UserId;
            request.Layers = Enumerable.Range(0, 2).Select(i => CreateSaveLayerRequest(i, LayerType.Point, isVisible: true)).ToList();
            request.Layers.AddRange(layersToUpdate.Select(l =>
            {
                var layerRequest = CreateSaveLayerRequest(l.Index, l.Type, isVisible: true);
                layerRequest.Id = l.Id;
                return layerRequest;
            }));

            await service.SaveAsync(request);

            Assert.Equal(request.Name, existingMap.Name);
            Assert.Equal(request.Description, existingMap.Description);

            Assert.All(request.Layers.Where(requestLayer => requestLayer.Id.HasValue),
                layerRequest =>
                    Assert.Contains(layersToUpdate,
                        layer =>
                            layer.Id == layerRequest.Id &&
                            layer.Name == layerRequest.Name &&
                            layer.Description == layerRequest.Description &&
                            layer.Index == layerRequest.Index &&
                            layer.IsVisible == layerRequest.IsVisible &&
                            layer.StyleOptions == layerRequest.StyleOptions));

            layersRepositoryMock.Verify(x => x.DeleteByMapIdAsync(existingMap.Id,
                    It.Is<IEnumerable<Guid>>(layerIds =>
                        layerIds.Count() == 2 &&
                        layerIds.All(layerId => layersToUpdate.Any(layer => layer.Id == layerId)))),
                Times.Once);

            layersRepositoryMock.Verify(x => x.AddAsync(It.Is<IEnumerable<Layer>>(layers =>
                    layers.Count() == 2 &&
                    request.Layers.Where(requestLayer => !requestLayer.Id.HasValue).All(requestLayer =>
                        layers.Any(layer =>
                            layer.Name == requestLayer.Name &&
                            layer.Description == requestLayer.Description &&
                            layer.Data == requestLayer.Data &&
                            layer.StyleOptions == requestLayer.StyleOptions &&
                            layer.Index == requestLayer.Index &&
                            layer.Type == (LayerType)requestLayer.Type &&
                            layer.MapId == existingMap.Id)))),
                Times.Once);
        }

        private static Layer CreateLayer(Map map, int index) => new Layer
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Description = Guid.NewGuid().ToString(),
            MapId = map.Id,
            Map = map,
            Data = Guid.NewGuid().ToString(),
            StyleOptions = Guid.NewGuid().ToString(),
            Index = index
        };

        private static Map CreateMap() => new Map
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Description = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid()
        };

        private static SaveLayerRequest CreateSaveLayerRequest(int index, LayerType type = LayerType.None, bool isVisible = false) => new SaveLayerRequest
        {
            Name = Guid.NewGuid().ToString(),
            Description = Guid.NewGuid().ToString(),
            Data = Guid.NewGuid().ToString(),
            StyleOptions = Guid.NewGuid().ToString(),
            Index = index,
            IsVisible = isVisible,
            Type = (int)type
        };

        private static SaveMapRequest CreateSaveMapRequest() => new SaveMapRequest
        {
            Name = Guid.NewGuid().ToString(),
            Description = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid()
        };
    }
}

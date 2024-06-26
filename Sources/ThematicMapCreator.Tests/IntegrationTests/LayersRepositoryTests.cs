﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests
{
    public class LayersRepositoryTests : EfRepositoryTests, IAsyncLifetime
    {
        private readonly Map map;
        private readonly User user;

        public LayersRepositoryTests()
        {
            user = CreateUser();
            map = CreateMap(user.Id);
        }

        [Fact]
        public async Task Add_ManyEntities_Success()
        {
            Layer[] layers = { CreateLayer(map.Id), CreateLayer(map.Id) };

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                foreach (var layer in layers)
                {
                    await repository.AddAsync(layer);
                }

                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();

                foreach (var layer in layers)
                {
                    var storedLayer = await repository.GetAsync(layer.Id);
                    Assert.NotNull(storedLayer);
                    Assert.Equal(layer.Id, storedLayer.Id);
                    Assert.Equal(layer.Name, storedLayer.Name);
                    Assert.Equal(layer.Description, storedLayer.Description);
                    Assert.Equal(layer.MapId, storedLayer.MapId);
                    Assert.Equal(layer.Index, storedLayer.Index);
                    Assert.Equal(layer.IsVisible, storedLayer.IsVisible);
                    Assert.Equal(layer.Data, storedLayer.Data);
                    Assert.Equal(layer.StyleOptions, storedLayer.StyleOptions);
                }

                await unitOfWork.CommitAsync();
            }
        }

        [Fact]
        public async Task Add_NewEntity_Success()
        {
            var layer = CreateLayer(map.Id);

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                await repository.AddAsync(layer);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                var storedLayer = await repository.GetAsync(layer.Id);
                await unitOfWork.CommitAsync();

                Assert.NotNull(storedLayer);
                Assert.Equal(layer.Id, storedLayer.Id);
                Assert.Equal(layer.Name, storedLayer.Name);
                Assert.Equal(layer.Description, storedLayer.Description);
                Assert.Equal(layer.MapId, storedLayer.MapId);
                Assert.Equal(layer.Index, storedLayer.Index);
                Assert.Equal(layer.IsVisible, storedLayer.IsVisible);
                Assert.Equal(layer.Data, storedLayer.Data);
                Assert.Equal(layer.StyleOptions, storedLayer.StyleOptions);
            }
        }

        [Fact]
        public async Task Delete_EntityExists_Success()
        {
            var layer = CreateLayer(map.Id);

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                await repository.AddAsync(layer);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                await repository.DeleteAsync(layer.Id);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                var storedLayer = await repository.GetAsync(layer.Id);
                await unitOfWork.CommitAsync();

                Assert.Null(storedLayer);
            }
        }

        [Fact]
        public async Task DeleteByMapId_WithExcluded_Success()
        {
            Layer[] layers = { CreateLayer(map.Id), CreateLayer(map.Id) };

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                foreach (var layer in layers)
                {
                    await repository.AddAsync(layer);
                }

                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                await repository.DeleteByMapIdAsync(map.Id, new[] { layers.First().Id });
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                List<Layer> storedLayers = await repository.GetByMapIdAsync(map.Id);
                await unitOfWork.CommitAsync();

                Assert.NotEmpty(storedLayers);
                Assert.Contains(storedLayers, layer => layer.Id == layers.First().Id);
                Assert.All(layers.Skip(1), layer => Assert.DoesNotContain(storedLayers, storedLayer => layer.Id == storedLayer.Id));
            }
        }

        [Fact]
        public async Task DeleteByMapId_WithoutExcluded_Success()
        {
            Layer[] layers = { CreateLayer(map.Id), CreateLayer(map.Id) };

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                foreach (var layer in layers)
                {
                    await repository.AddAsync(layer);
                }

                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                await repository.DeleteByMapIdAsync(map.Id);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                List<Layer> storedLayers = await repository.GetByMapIdAsync(map.Id);
                await unitOfWork.CommitAsync();

                Assert.Empty(storedLayers);
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetByMapId_Empty_Success()
        {
            await using var unitOfWork = await UnitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<ILayersRepository>();
            List<Layer> storedLayers = await repository.GetByMapIdAsync(map.Id);
            await unitOfWork.CommitAsync();

            Assert.Empty(storedLayers);
        }

        [Fact]
        public async Task GetByMapId_EntitiesAdded_Success()
        {
            Layer[] layers = { CreateLayer(map.Id), CreateLayer(map.Id) };

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                foreach (var layer in layers)
                {
                    await repository.AddAsync(layer);
                }

                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                List<Layer> storedLayers = await repository.GetByMapIdAsync(map.Id);
                await unitOfWork.CommitAsync();

                Assert.NotEmpty(storedLayers);
                Assert.Equal(layers.Length, storedLayers.Count);
                Assert.All(layers, map => Assert.Contains(storedLayers, storedMap => map.Id == storedMap.Id));
            }
        }

        public async Task InitializeAsync()
        {
            await using var unitOfWork = await UnitOfWorkFactory.CreateAsync();
            var usersRepository = unitOfWork.GetRepository<IUsersRepository>();
            var mapsRepository = unitOfWork.GetRepository<IMapsRepository>();
            await usersRepository.AddAsync(user);
            await mapsRepository.AddAsync(map);
            await unitOfWork.CommitAsync();
        }

        [Fact]
        public async Task Update_WithSpecialMethod_Success()
        {
            var layer = CreateLayer(map.Id);

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                await repository.AddAsync(layer);
                await unitOfWork.CommitAsync();
            }

            var updatedLayer = CreateLayer(map.Id);
            updatedLayer.Id = layer.Id;
            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                await repository.UpdateAsync(updatedLayer);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<ILayersRepository>();
                var storedLayer = await repository.GetAsync(layer.Id);
                await unitOfWork.CommitAsync();

                Assert.NotNull(storedLayer);
                Assert.Equal(updatedLayer.Id, storedLayer.Id);
                Assert.Equal(updatedLayer.Name, storedLayer.Name);
                Assert.Equal(updatedLayer.Description, storedLayer.Description);
                Assert.Equal(updatedLayer.MapId, storedLayer.MapId);
                Assert.Equal(updatedLayer.Index, storedLayer.Index);
                Assert.Equal(updatedLayer.IsVisible, storedLayer.IsVisible);
                Assert.Equal(updatedLayer.Data, storedLayer.Data);
                Assert.Equal(updatedLayer.StyleOptions, storedLayer.StyleOptions);
            }
        }

        private static Layer CreateLayer(Guid mapId) => new()
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Description = Guid.NewGuid().ToString(),
            MapId = mapId,
            Data = Guid.NewGuid().ToString(),
            StyleOptions = Guid.NewGuid().ToString(),
            IsVisible = true,
            Type = LayerType.Line
        };

        private static Map CreateMap(Guid userId) => new()
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Description = Guid.NewGuid().ToString(),
            UserId = userId
        };

        private static User CreateUser() => new()
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Email = Guid.NewGuid().ToString(),
            PasswordHash = Guid.NewGuid().ToString(),
        };
    }
}

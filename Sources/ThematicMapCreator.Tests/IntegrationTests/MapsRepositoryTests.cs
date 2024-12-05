using System;
using System.Threading.Tasks;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests;

public sealed class MapsRepositoryTests : EfRepositoryTests, IAsyncLifetime
{
    private readonly User _user;

    public MapsRepositoryTests() => _user = CreateUser();

    [Fact]
    public async Task Add_NewEntity_Success()
    {
        var map = CreateMap(_user.Id);

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            await repository.AddAsync(map);
            await unitOfWork.CommitAsync();
        }

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            var storedMap = await repository.GetAsync(map.Id);
            await unitOfWork.CommitAsync();

            Assert.NotNull(storedMap);
            Assert.Equal(map.Id, storedMap.Id);
            Assert.Equal(map.Name, storedMap.Name);
            Assert.Equal(map.Description, storedMap.Description);
            Assert.Equal(map.UserId, storedMap.UserId);
        }
    }

    [Fact]
    public async Task Delete_EntityExists_Success()
    {
        var map = CreateMap(_user.Id);

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            await repository.AddAsync(map);
            await unitOfWork.CommitAsync();
        }

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            await repository.DeleteAsync(map.Id);
            await unitOfWork.CommitAsync();
        }

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            var storedMap = await repository.GetAsync(map.Id);
            await unitOfWork.CommitAsync();

            Assert.Null(storedMap);
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Exists_Empty_ReturnsFalse()
    {
        var map = CreateMap(_user.Id);

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            await repository.AddAsync(map);
            await unitOfWork.CommitAsync();
        }

        var mapName = Guid.NewGuid().ToString();
        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            var exists = await repository.ExistsAsync(_user.Id, mapName);
            await unitOfWork.CommitAsync();

            Assert.False(exists);
        }
    }

    [Fact]
    public async Task Exists_EntityAdded_ReturnsTrue()
    {
        var map = CreateMap(_user.Id);

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            await repository.AddAsync(map);
            await unitOfWork.CommitAsync();
        }

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            var exists = await repository.ExistsAsync(_user.Id, map.Name);
            await unitOfWork.CommitAsync();

            Assert.True(exists);
        }
    }

    [Fact]
    public async Task GetByUserId_Empty_Success()
    {
        await using var unitOfWork = await UnitOfWorkFactory.CreateAsync();
        var repository = unitOfWork.GetRepository<IMapsRepository>();
        var storedMaps = await repository.GetByUserIdAsync(_user.Id);
        await unitOfWork.CommitAsync();

        Assert.Empty(storedMaps);
    }

    [Fact]
    public async Task GetByUserId_EntitiesAdded_Success()
    {
        var maps = new[] { CreateMap(_user.Id), CreateMap(_user.Id) };

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            foreach (var map in maps)
            {
                await repository.AddAsync(map);
            }

            await unitOfWork.CommitAsync();
        }

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            var storedMaps = await repository.GetByUserIdAsync(_user.Id);
            await unitOfWork.CommitAsync();

            Assert.NotEmpty(storedMaps);
            Assert.Equal(maps.Length, storedMaps.Count);
            Assert.All(maps, map => Assert.Contains(storedMaps, storedMap => map.Id == storedMap.Id));
        }
    }

    public async Task InitializeAsync()
    {
        await using var unitOfWork = await UnitOfWorkFactory.CreateAsync();
        var repository = unitOfWork.GetRepository<IUsersRepository>();
        await repository.AddAsync(_user);
        await unitOfWork.CommitAsync();
    }

    [Fact]
    public async Task Update_WithSpecialMethod_Success()
    {
        var map = CreateMap(_user.Id);

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            await repository.AddAsync(map);
            await unitOfWork.CommitAsync();
        }

        var updatedMap = CreateMap(_user.Id);
        updatedMap.Id = map.Id;
        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            await repository.UpdateAsync(updatedMap);
            await unitOfWork.CommitAsync();
        }

        await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
        {
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            var storedMap = await repository.GetAsync(map.Id);
            await unitOfWork.CommitAsync();

            Assert.NotNull(storedMap);
            Assert.Equal(updatedMap.Id, storedMap.Id);
            Assert.Equal(updatedMap.Name, storedMap.Name);
            Assert.Equal(updatedMap.Description, storedMap.Description);
            Assert.Equal(updatedMap.UserId, storedMap.UserId);
        }
    }

    private static Map CreateMap(Guid userId) => new()
    {
        Id = Guid.NewGuid(),
        Name = Guid.NewGuid().ToString(),
        Description = Guid.NewGuid().ToString(),
        UserId = userId,
    };

    private static User CreateUser() => new()
    {
        Id = Guid.NewGuid(),
        Name = Guid.NewGuid().ToString(),
        Email = Guid.NewGuid().ToString(),
        PasswordHash = Guid.NewGuid().ToString(),
    };
}

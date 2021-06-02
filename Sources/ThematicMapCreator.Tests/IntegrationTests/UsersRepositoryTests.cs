using System;
using System.Threading.Tasks;
using Core.Dal;
using Core.Dal.EntityFramework;
using Core.Dal.EntityFramework.Extensions;
using Core.Dal.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using ThematicMapCreator.Host.Persistence.Contexts;
using ThematicMapCreator.Host.Persistence.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests
{
    public class UsersRepositoryTests : IDisposable
    {
        private const string Tag = "test";
        private readonly ServiceProvider provider;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public UsersRepositoryTests()
        {
            var services = new ServiceCollection()
                .AddUnitOfWorkFactory<EfUnitOfWorkFactory>()
                .AddDbContextFactory<ThematicMapDbContext>(Tag,
                    builder => builder
                        .UseInMemoryDatabase(Tag)
                        .ConfigureWarnings(warn => warn.Ignore(InMemoryEventId.TransactionIgnoredWarning)))
                .AddRepository<IUsersRepository, UsersRepository>();

            provider = services.BuildServiceProvider();
            unitOfWorkFactory = provider.GetRequiredService<IUnitOfWorkFactory>();

            var contextFactory = provider.GetRequiredService<IDbContextFactory>();
            using var context = contextFactory.Create();
            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task Add_NewEntity_Success()
        {
            var user = CreateUser();

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync(Tag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.AddAsync(user);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                await unitOfWork.CommitAsync();

                Assert.Equal(user.Id, storedUser.Id);
                Assert.Equal(user.Name, storedUser.Name);
            }
        }

        [Fact]
        public async Task Delete_EntityExists_Success()
        {
            var user = CreateUser();

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync(Tag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.AddAsync(user);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync(Tag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.DeleteAsync(user.Id);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                await unitOfWork.CommitAsync();

                Assert.Null(storedUser);
            }
        }

        public void Dispose()
        {
            var contextFactory = provider.GetRequiredService<IDbContextFactory>();
            using var context = contextFactory.Create();
            context.Database.EnsureDeleted();

            provider.Dispose();
        }

        [Fact]
        public async Task Update_WithoutSpecialMethod_Success()
        {
            var user = CreateUser();

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync(Tag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.AddAsync(user);
                await unitOfWork.CommitAsync();
            }

            var newName = Guid.NewGuid().ToString();
            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync(Tag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                storedUser.Name = newName;
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                await unitOfWork.CommitAsync();

                Assert.Equal(user.Id, storedUser.Id);
                Assert.Equal(newName, storedUser.Name);
            }
        }

        [Fact]
        public async Task Update_WithSpecialMethod_Success()
        {
            var user = CreateUser();

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync(Tag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.AddAsync(user);
                await unitOfWork.CommitAsync();
            }

            var updatedUser = CreateUser();
            updatedUser.Id = user.Id;
            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync(Tag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.UpdateAsync(updatedUser);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await unitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                await unitOfWork.CommitAsync();

                Assert.Equal(updatedUser.Id, storedUser.Id);
                Assert.Equal(updatedUser.Name, storedUser.Name);
            }
        }

        private User CreateUser() => new User
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Email = Guid.NewGuid().ToString()
        };
    }
}

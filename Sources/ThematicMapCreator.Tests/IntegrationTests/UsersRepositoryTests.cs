using System;
using System.Threading.Tasks;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests
{
    public class UsersRepositoryTests : EfRepositoryTests
    {
        [Fact]
        public async Task Add_NewEntity_Success()
        {
            var user = CreateUser();

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync(DbTag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.AddAsync(user);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                await unitOfWork.CommitAsync();

                Assert.NotNull(storedUser);
                Assert.Equal(user.Id, storedUser.Id);
                Assert.Equal(user.Name, storedUser.Name);
            }
        }

        [Fact]
        public async Task Delete_EntityExists_Success()
        {
            var user = CreateUser();

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync(DbTag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.AddAsync(user);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync(DbTag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.DeleteAsync(user.Id);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                await unitOfWork.CommitAsync();

                Assert.Null(storedUser);
            }
        }

        [Fact]
        public async Task Update_WithoutSpecialMethod_Success()
        {
            var user = CreateUser();

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync(DbTag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.AddAsync(user);
                await unitOfWork.CommitAsync();
            }

            var newName = Guid.NewGuid().ToString();
            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync(DbTag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                storedUser.Name = newName;
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
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

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync(DbTag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.AddAsync(user);
                await unitOfWork.CommitAsync();
            }

            var updatedUser = CreateUser();
            updatedUser.Id = user.Id;
            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync(DbTag))
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                await repository.UpdateAsync(updatedUser);
                await unitOfWork.CommitAsync();
            }

            await using (var unitOfWork = await UnitOfWorkFactory.CreateAsync())
            {
                var repository = unitOfWork.GetRepository<IUsersRepository>();
                var storedUser = await repository.GetAsync(user.Id);
                await unitOfWork.CommitAsync();

                Assert.NotNull(storedUser);
                Assert.Equal(updatedUser.Id, storedUser.Id);
                Assert.Equal(updatedUser.Name, storedUser.Name);
            }
        }

        private static User CreateUser() => new()
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Email = Guid.NewGuid().ToString(),
            PasswordHash = Guid.NewGuid().ToString(),
        };
    }
}

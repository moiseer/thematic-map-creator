using System;
using Core.Dal;
using Core.Dal.EntityFramework;
using Core.Dal.EntityFramework.Extensions;
using Core.Dal.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ThematicMapCreator.Domain.Repositories;
using ThematicMapCreator.Host.Persistence.Contexts;
using ThematicMapCreator.Host.Persistence.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests
{
    [Collection("EfRepositoryTests")]
    public abstract class EfRepositoryTests : IDisposable
    {
        protected const string DbTag = "test";
        protected readonly IUnitOfWorkFactory UnitOfWorkFactory;

        private readonly ServiceProvider provider;

        protected EfRepositoryTests()
        {
            var services = new ServiceCollection()
                .AddUnitOfWorkFactory<EfUnitOfWorkFactory>()
                .AddDbContextFactory<ThematicMapDbContext>(DbTag, builder => builder.UseSqlite("Data Source=Test.db"))
                .AddRepository<IUsersRepository, UsersRepository>()
                .AddRepository<IMapsRepository, MapsRepository>()
                .AddRepository<ILayersRepository, LayersRepository>();

            provider = services.BuildServiceProvider();
            UnitOfWorkFactory = provider.GetRequiredService<IUnitOfWorkFactory>();

            var contextFactory = provider.GetRequiredService<IDbContextFactory>();
            using var context = contextFactory.Create();
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            var contextFactory = provider.GetRequiredService<IDbContextFactory>();
            using var context = contextFactory.Create();
            context.Database.EnsureDeleted();

            provider.Dispose();
        }
    }
}

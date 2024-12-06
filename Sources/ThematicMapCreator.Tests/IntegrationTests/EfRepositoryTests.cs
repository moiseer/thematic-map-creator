using System;
using Core.Dal;
using Core.Dal.EntityFramework;
using Core.Dal.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ThematicMapCreator.Domain.Repositories;
using ThematicMapCreator.Host.Persistence.Contexts;
using ThematicMapCreator.Host.Persistence.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests;

[Collection("DatabaseTests")]
public abstract class EfRepositoryTests : IDisposable
{
    protected readonly IUnitOfWorkFactory UnitOfWorkFactory;

    private readonly ServiceProvider _provider;

    protected EfRepositoryTests()
    {
        _provider = new ServiceCollection()
            .AddEfUnitOfWorkFactory<ThematicMapDbContext>(builder => builder.UseSqlite("Data Source=Test.db"))
            .AddRepository<IUsersRepository, UsersRepository>()
            .AddRepository<IMapsRepository, MapsRepository>()
            .AddRepository<ILayersRepository, LayersRepository>()
            .BuildServiceProvider();

        UnitOfWorkFactory = _provider.GetRequiredService<IUnitOfWorkFactory>();

        var contextFactory = _provider.GetRequiredService<IDbContextFactory>();
        using var context = contextFactory.Create();
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        var contextFactory = _provider.GetRequiredService<IDbContextFactory>();
        using var context = contextFactory.Create();
        context.Database.EnsureDeleted();

        _provider.Dispose();
    }
}

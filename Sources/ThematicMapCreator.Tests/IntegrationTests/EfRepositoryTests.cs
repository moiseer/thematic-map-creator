using System.Threading.Tasks;
using Core.Dal;
using Core.Dal.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ThematicMapCreator.Domain.Repositories;
using ThematicMapCreator.Host.Persistence.Contexts;
using ThematicMapCreator.Host.Persistence.Repositories;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests;

[Collection("PostgreSqlTests")]
public abstract class EfRepositoryTests : IAsyncLifetime
{
    protected readonly IUnitOfWorkFactory UnitOfWorkFactory;

    private readonly ServiceProvider _provider;

    protected EfRepositoryTests(PostgreSqlFixture fixture)
    {
        _provider = new ServiceCollection()
            .AddEfUnitOfWorkFactory<ThematicMapDbContext>(builder => builder.UseNpgsql(fixture.GetConnectionString("test")))
            .AddRepository<IUsersRepository, UsersRepository>()
            .AddRepository<IMapsRepository, MapsRepository>()
            .AddRepository<ILayersRepository, LayersRepository>()
            .BuildServiceProvider();

        UnitOfWorkFactory = _provider.GetRequiredService<IUnitOfWorkFactory>();
    }

    /// <inheritdoc/>
    public virtual async Task DisposeAsync()
    {
        var contextFactory = _provider.GetRequiredService<IDbContextFactory<ThematicMapDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureDeletedAsync();

        await _provider.DisposeAsync();
    }

    /// <inheritdoc/>
    public virtual async Task InitializeAsync()
    {
        var contextFactory = _provider.GetRequiredService<IDbContextFactory<ThematicMapDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

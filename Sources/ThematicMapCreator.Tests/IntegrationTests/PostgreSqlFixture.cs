using System.Threading.Tasks;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder().Build();

    /// <inheritdoc/>
    public async Task DisposeAsync() => await _container.DisposeAsync();

    public string GetConnectionString(string database)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Database = database,
        };

        return connectionStringBuilder.ConnectionString;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync() => await _container.StartAsync();
}

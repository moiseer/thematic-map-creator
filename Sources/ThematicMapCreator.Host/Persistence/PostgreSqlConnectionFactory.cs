using System.Data.Common;
using Core.Dal.Dapper;
using Npgsql;

namespace ThematicMapCreator.Host.Persistence;

public sealed class PostgreSqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public PostgreSqlConnectionFactory(string connectionString) => _connectionString = connectionString;

    /// <inheritdoc/>
    public DbConnection Create() => new NpgsqlConnection(_connectionString);
}

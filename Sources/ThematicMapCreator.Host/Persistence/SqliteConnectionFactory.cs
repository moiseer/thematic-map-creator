using System.Data.Common;
using Core.Dal.Dapper;
using Microsoft.Data.Sqlite;

namespace ThematicMapCreator.Host.Persistence;

public sealed class SqliteConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString) => _connectionString = connectionString;

    /// <inheritdoc/>
    public DbConnection Create() => new SqliteConnection(_connectionString);
}

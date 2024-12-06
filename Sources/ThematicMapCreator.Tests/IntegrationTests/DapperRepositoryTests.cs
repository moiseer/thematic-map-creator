using System;
using System.Data.Common;
using System.Threading.Tasks;
using Core.Dal;
using Core.Dal.Dapper;
using Core.Dal.Dapper.Extensions;
using Core.Dal.Repositories;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using ThematicMapCreator.Host.Persistence;
using Xunit;

namespace ThematicMapCreator.Tests.IntegrationTests;

[Collection("DatabaseTests")]
public sealed class DapperUnitOfWorkTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public DapperUnitOfWorkTests()
    {
        _provider = new ServiceCollection()
            .AddDapperUnitOfWorkFactory()
            .AddSingleton<IConnectionFactory>(_ => new SqliteConnectionFactory("Data Source=Test.db"))
            .AddRepository<IRepository, TestRepository>()
            .BuildServiceProvider();

        _unitOfWorkFactory = _provider.GetRequiredService<IUnitOfWorkFactory>();
    }

    /// <inheritdoc/>
    public void Dispose() => _provider.Dispose();

    [Fact]
    public async Task TestConnection()
    {
        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync();
        var repository = (TestRepository)unitOfWork.GetRepository<IRepository>();
        var exception = await Record.ExceptionAsync(async () => await repository.TestAsync());
        await unitOfWork.CommitAsync();

        Assert.Null(exception);
    }

    private sealed class TestRepository : DapperRepository
    {
        public TestRepository(DbConnection connection, DbTransaction transaction)
            : base(connection, transaction)
        {
        }

        public async Task TestAsync() => await Connection.ExecuteAsync("SELECT 1;", param: null, Transaction);
    }
}

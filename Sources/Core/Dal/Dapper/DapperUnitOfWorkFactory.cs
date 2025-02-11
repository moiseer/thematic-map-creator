namespace Core.Dal.Dapper;

public sealed class DapperUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IServiceProvider _serviceProvider;

    public DapperUnitOfWorkFactory(IConnectionFactory connectionFactory, IServiceProvider serviceProvider)
    {
        _connectionFactory = connectionFactory;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public IUnitOfWork Create()
    {
        var connection = _connectionFactory.Create();
        connection.Open();
        var transaction = connection.BeginTransaction();
        return new DapperUnitOfWork(connection, transaction, _serviceProvider);
    }

    /// <inheritdoc/>
    public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
    {
        var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        var transaction = await connection.BeginTransactionAsync(cancellationToken);
        return new DapperUnitOfWork(connection, transaction, _serviceProvider);
    }
}

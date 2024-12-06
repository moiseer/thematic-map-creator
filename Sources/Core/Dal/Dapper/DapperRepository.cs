using System.Data.Common;
using Core.Dal.Repositories;

namespace Core.Dal.Dapper;

public abstract class DapperRepository : IRepository
{
    protected readonly DbConnection Connection;
    protected readonly DbTransaction Transaction;

    protected DapperRepository(DbConnection connection, DbTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }

    public delegate TRepository Factory<out TRepository>(DbConnection connection, DbTransaction transaction)
        where TRepository : IRepository;
}

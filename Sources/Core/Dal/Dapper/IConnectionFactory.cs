using System.Data.Common;

namespace Core.Dal.Dapper;

public interface IConnectionFactory
{
    DbConnection Create();
}

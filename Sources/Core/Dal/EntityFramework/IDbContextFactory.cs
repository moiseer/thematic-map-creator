using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework;

public interface IDbContextFactory
{
    public DbContext Create();
}

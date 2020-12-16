using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework
{
    public interface IDbContextFactory
    {
        public string Tag { get; }
        public DbContext Create();
    }
}

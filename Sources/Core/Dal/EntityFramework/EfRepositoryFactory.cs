using Core.Dal.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework
{
    public delegate TRepository EfRepositoryFactory<out TRepository>(DbContext context)
        where TRepository : IRepository;
}

using System;
using Core.Dal.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Dal.EntityFramework
{
    public abstract class EfRepository : IRepository
    {
        protected readonly DbContext Context;

        protected EfRepository(DbContext context)
        {
            Context = context;
        }
    }
}

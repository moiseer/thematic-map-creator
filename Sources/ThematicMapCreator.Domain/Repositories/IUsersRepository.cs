using System;
using Core.Dal.Repositories;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Repositories
{
    public interface IUsersRepository : IAsyncCrudRepository<User, Guid>
    {
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;
using Z.EntityFramework.Plus;

namespace ThematicMapCreator.Host.Persistence.Repositories
{
    public class UsersRepository : EfCrudRepository<User, Guid>, IUsersRepository
    {
        public UsersRepository(DbContext context) : base(context)
        {
        }

        public override async Task UpdateAsync(User entity, CancellationToken cancellationToken = default) =>
            await Context.Set<User>().Where(user => user.Id == entity.Id).UpdateAsync(
                user => new User
                {
                    Name = entity.Name,
                    Email = entity.Email,
                    PasswordHash = entity.PasswordHash
                },
                cancellationToken);
    }
}

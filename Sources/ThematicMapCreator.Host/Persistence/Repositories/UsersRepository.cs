using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Host.Persistence.Repositories;

public sealed class UsersRepository : EfCrudRepository<User, Guid>, IUsersRepository
{
    public UsersRepository(DbContext context)
        : base(context)
    {
    }

    public override async Task UpdateAsync(User entity, CancellationToken ct = default) =>
        await Context.Set<User>().Where(user => user.Id == entity.Id).ExecuteUpdateAsync(
            calls => calls
                .SetProperty(user => user.Name, entity.Name)
                .SetProperty(user => user.Email, entity.Email)
                .SetProperty(user => user.PasswordHash, entity.PasswordHash),
            ct);
}

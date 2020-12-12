using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dal;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public UsersService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<List<Map>> GetMapsAsync(Guid id)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync();
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            List<Map> layers = await repository.GetByUserIdAsync(id)
                .ThrowOnEmptyAsync(() => new TmcException(TmcError.User.NotFound));
            await unitOfWork.CommitAsync();

            return layers;
        }
    }
}

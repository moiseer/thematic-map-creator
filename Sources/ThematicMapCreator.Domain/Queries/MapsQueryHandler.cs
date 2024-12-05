using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using MediatR;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Queries;

public sealed class MapsQueryHandler : IRequestHandler<MapsQuery, IEnumerable<Map>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public MapsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<IEnumerable<Map>> Handle(MapsQuery request, CancellationToken cancellationToken)
    {
        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync(cancellationToken);
        var repository = unitOfWork.GetRepository<IMapsRepository>();
        var maps = await repository.GetByUserIdAsync(request.UserId);
        await unitOfWork.CommitAsync(cancellationToken);

        return maps;
    }
}

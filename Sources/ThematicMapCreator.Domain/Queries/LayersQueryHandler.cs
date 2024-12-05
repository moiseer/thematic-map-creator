using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using MediatR;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Queries;

public sealed class LayersQueryHandler : IRequestHandler<LayersQuery, IEnumerable<Layer>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public LayersQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<IEnumerable<Layer>> Handle(LayersQuery request, CancellationToken cancellationToken)
    {
        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync(cancellationToken);
        var repository = unitOfWork.GetRepository<ILayersRepository>();
        var layers = await repository.GetByMapIdAsync(request.MapId);
        await unitOfWork.CommitAsync(cancellationToken);

        return layers;
    }
}

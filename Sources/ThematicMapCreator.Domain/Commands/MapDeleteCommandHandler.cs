using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using MediatR;
using Microsoft.Extensions.Logging;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Commands;

public sealed class MapDeleteCommandHandler : IRequestHandler<MapDeleteCommand>
{
    private readonly ILogger<MapDeleteCommandHandler> logger;
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    public MapDeleteCommandHandler(ILogger<MapDeleteCommandHandler> logger, IUnitOfWorkFactory unitOfWorkFactory)
    {
        this.logger = logger;
        this.unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(MapDeleteCommand request, CancellationToken cancellationToken)
    {
        await using var unitOfWork = await unitOfWorkFactory.CreateAsync(cancellationToken);
        var layerRepository = unitOfWork.GetRepository<ILayersRepository>();
        var mapRepository = unitOfWork.GetRepository<IMapsRepository>();
        await layerRepository.DeleteByMapIdAsync(request.MapId);
        await mapRepository.DeleteAsync(request.MapId, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Map {MapId} deleted", request.MapId);
    }
}
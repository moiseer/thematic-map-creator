using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using MediatR;
using Microsoft.Extensions.Logging;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Commands;

public sealed class MapDeleteCommandHandler : IRequestHandler<MapDeleteCommand>
{
    private readonly ILogger<MapDeleteCommandHandler> _logger;
    private readonly IPublisher _publisher;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public MapDeleteCommandHandler(ILogger<MapDeleteCommandHandler> logger, IPublisher publisher, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _logger = logger;
        _publisher = publisher;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(MapDeleteCommand request, CancellationToken cancellationToken)
    {
        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync(cancellationToken);
        var layerRepository = unitOfWork.GetRepository<ILayersRepository>();
        var mapRepository = unitOfWork.GetRepository<IMapsRepository>();
        await layerRepository.DeleteByMapIdAsync(request.MapId);
        await mapRepository.DeleteAsync(request.MapId, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        await _publisher.Publish(new MapDeleteNotification(request.MapId), cancellationToken);
        _logger.LogInformation("Map {MapId} deleted", request.MapId);
    }
}

﻿using Core.Dal;
using MediatR;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;
using ThematicMapCreator.Domain.Models;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Queries;

public sealed class MapDetailsQueryHandler : IRequestHandler<MapDetailsQuery, Map>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public MapDetailsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) => _unitOfWorkFactory = unitOfWorkFactory;

    public async Task<Map> Handle(MapDetailsQuery request, CancellationToken cancellationToken)
    {
        await using var unitOfWork = await _unitOfWorkFactory.CreateAsync(cancellationToken);
        var repository = unitOfWork.GetRepository<IMapsRepository>();
        var map = await repository.GetAsync(request.MapId, cancellationToken)
            .ThrowOnEmptyAsync(() => new TmcException(TmcError.Map.NotFound));
        await unitOfWork.CommitAsync(cancellationToken);

        return map;
    }
}

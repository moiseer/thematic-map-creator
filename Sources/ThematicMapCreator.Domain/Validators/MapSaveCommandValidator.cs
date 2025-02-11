using Core.Dal;
using FluentValidation;
using ThematicMapCreator.Domain.Commands;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Validators;

public class MapSaveCommandValidator : AbstractValidator<MapSaveCommand>
{
    public MapSaveCommandValidator(IUnitOfWorkFactory unitOfWorkFactory)
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(TmcError.Map.NameRequired);
        RuleFor(x => x.Name).MinimumLength(3).WithErrorCode(TmcError.Map.ShortName);
        RuleFor(x => x.Name).MaximumLength(64).WithErrorCode(TmcError.Map.LongName);
        RuleFor(x => x.Description).MaximumLength(1024).WithErrorCode(TmcError.Map.LongDescription);
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode(TmcError.Map.UserRequired);

        RuleFor(x => x.Layers).Must(IsUniqueNames).WithErrorCode(TmcError.Layer.NotUniqueName);
        RuleForEach(x => x.Layers).SetValidator(new LayerValidator());

        RuleFor(x => x.Name)
            .MustAsync(async (map, _, ct) => await IsUniqueNameAsync(map, unitOfWorkFactory, ct))
            .WithErrorCode(TmcError.Map.NotUniqueName);
    }

    private static async Task<bool> IsUniqueNameAsync(MapSaveCommand map, IUnitOfWorkFactory unitOfWorkFactory, CancellationToken cancellationToken)
    {
        await using var unitOfWork = await unitOfWorkFactory.CreateAsync(cancellationToken);
        var repository = unitOfWork.GetRepository<IMapsRepository>();
        var exists = await repository.ExistsAsync(map.UserId, map.Name!);
        await unitOfWork.CommitAsync(cancellationToken);
        return !exists;
    }

    private static bool IsUniqueNames(IEnumerable<MapSaveCommand.Layer> layers) =>
        !layers.GroupBy(layer => layer.Name?.Trim()).Any(group => group.Count() > 1);

    private sealed class LayerValidator : AbstractValidator<MapSaveCommand.Layer>
    {
        public LayerValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithErrorCode(TmcError.Layer.NameRequired);
            RuleFor(x => x.Name).MinimumLength(3).WithErrorCode(TmcError.Layer.ShortName);
            RuleFor(x => x.Name).MaximumLength(64).WithErrorCode(TmcError.Layer.LongName);
            RuleFor(x => x.Description).MaximumLength(1024).WithErrorCode(TmcError.Layer.LongDescription);
            RuleFor(x => x.Type).IsInEnum().WithErrorCode(TmcError.Layer.InvalidType);
            RuleFor(x => x.Index).GreaterThanOrEqualTo(0).WithErrorCode(TmcError.Layer.InvalidIndex);
            RuleFor(x => x.Data).NotEmpty().WithErrorCode(TmcError.Layer.DataRequired);
            RuleFor(x => x.StyleOptions).NotEmpty().WithErrorCode(TmcError.Layer.InvalidStyle);
        }
    }
}

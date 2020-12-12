using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Dal;
using FluentValidation;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Repositories;

namespace ThematicMapCreator.Domain.Validators
{
    public class SaveMapRequestValidator : AbstractValidator<SaveMapRequest>
    {
        public SaveMapRequestValidator(IValidator<SaveLayerRequest> layerValidator, IUnitOfWorkFactory unitOfWorkFactory)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(TmcError.Map.NameRequired);
            RuleFor(x => x.Name).MinimumLength(3).WithMessage(TmcError.Map.ShortName);
            RuleFor(x => x.Name).MaximumLength(64).WithMessage(TmcError.Map.LongName);
            RuleFor(x => x.Description).MaximumLength(1024).WithMessage(TmcError.Map.LongDescription);
            RuleFor(x => x.UserId).NotEmpty().WithMessage(TmcError.Map.UserRequired);

            RuleFor(x => x.Layers).Must(IsUniqueNames).WithMessage(TmcError.Layer.NotUniqueName);
            RuleForEach(x => x.Layers).SetValidator(layerValidator);

            RuleFor(x => x.Name)
                .MustAsync(async (map, name, ct) => await IsUniqueNameAsync(map, unitOfWorkFactory, ct))
                .WithMessage(TmcError.Map.NotUniqueName);
        }

        private static async Task<bool> IsUniqueNameAsync(SaveMapRequest map, IUnitOfWorkFactory unitOfWorkFactory, CancellationToken cancellationToken)
        {
            await using var unitOfWork = await unitOfWorkFactory.CreateAsync(cancellationToken);
            var repository = unitOfWork.GetRepository<IMapsRepository>();
            var exists = await repository.ExistsAsync(map.UserId, map.Name);
            await unitOfWork.CommitAsync(cancellationToken);
            return !exists;
        }

        private static bool IsUniqueNames(IEnumerable<SaveLayerRequest> layers) =>
            !layers.GroupBy(layer => layer.Name.Trim()).Any(group => group.Count() > 1);
    }
}

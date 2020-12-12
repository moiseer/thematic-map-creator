using FluentValidation;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Domain.Validators
{
    public class SaveLayerRequestValidator : AbstractValidator<SaveLayerRequest>
    {
        public SaveLayerRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(TmcError.Layer.NameRequired);
            RuleFor(x => x.Name).MinimumLength(3).WithMessage(TmcError.Layer.ShortName);
            RuleFor(x => x.Name).MaximumLength(64).WithMessage(TmcError.Layer.LongName);
            RuleFor(x => x.Description).MaximumLength(1024).WithMessage(TmcError.Layer.LongDescription);
            RuleFor(x => x.Type).Transform(type => (LayerType)type).IsInEnum().WithMessage(TmcError.Layer.InvalidType);
            RuleFor(x => x.Index).GreaterThanOrEqualTo(0).WithMessage(TmcError.Layer.InvalidIndex);
            RuleFor(x => x.Data).NotEmpty().WithMessage(TmcError.Layer.DataRequired);
            RuleFor(x => x.StyleOptions).NotEmpty().WithMessage(TmcError.Layer.InvalidStyle);
        }
    }
}

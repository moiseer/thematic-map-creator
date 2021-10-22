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
            RuleFor(x => x.Name).NotEmpty().WithErrorCode(TmcError.Layer.NameRequired);
            RuleFor(x => x.Name).MinimumLength(3).WithErrorCode(TmcError.Layer.ShortName);
            RuleFor(x => x.Name).MaximumLength(64).WithErrorCode(TmcError.Layer.LongName);
            RuleFor(x => x.Description).MaximumLength(1024).WithErrorCode(TmcError.Layer.LongDescription);
            RuleFor(x => x.Type).Transform(type => (LayerType)type).IsInEnum().WithErrorCode(TmcError.Layer.InvalidType);
            RuleFor(x => x.Index).GreaterThanOrEqualTo(0).WithErrorCode(TmcError.Layer.InvalidIndex);
            RuleFor(x => x.Data).NotEmpty().WithErrorCode(TmcError.Layer.DataRequired);
            RuleFor(x => x.StyleOptions).NotEmpty().WithErrorCode(TmcError.Layer.InvalidStyle);
        }
    }
}

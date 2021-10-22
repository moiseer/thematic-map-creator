using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using ThematicMapCreator.Domain.Exceptions;

namespace ThematicMapCreator.Domain.Extensions
{
    public static class ValidationExtensions
    {
        public static async Task ThrowOnErrorsAsync(this Task<ValidationResult> asyncResult)
        {
            var result = await asyncResult;
            if (!result.IsValid)
            {
                throw new TmcException(result.Errors.Select(error => error.ErrorCode));
            }
        }
    }
}

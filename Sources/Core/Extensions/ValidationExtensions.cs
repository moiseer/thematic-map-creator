using System.Linq;
using System.Threading.Tasks;
using Core.Exceptions;
using FluentValidation;

namespace Core.Extensions
{
    public static class ValidationExtensions
    {
        public static async Task ThrowOnErrorsAsync<T>(this IValidator<T> validator, T instance)
        {
            var result = await validator.ValidateAsync(instance);
            if (!result.IsValid)
            {
                throw new TmcException(result.Errors.Select(error => error.ErrorMessage));
            }
        }
    }
}

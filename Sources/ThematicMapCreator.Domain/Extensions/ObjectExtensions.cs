using System;
using System.Threading.Tasks;

namespace ThematicMapCreator.Domain.Extensions;

public static class ObjectExtensions
{
    public static T ThrowOnEmpty<T>(this T? value, Func<Exception> exceptionFactory)
    {
        if (value is null)
        {
            throw exceptionFactory();
        }

        return value;
    }

    public static async Task<T> ThrowOnEmptyAsync<T>(this Task<T?> asyncValue, Func<Exception> exceptionFactory)
    {
        var value = await asyncValue;
        if (value is null)
        {
            throw exceptionFactory();
        }

        return value;
    }
}

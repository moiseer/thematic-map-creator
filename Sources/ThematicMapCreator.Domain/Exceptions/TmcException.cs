using System;
using System.Linq;

namespace ThematicMapCreator.Domain.Exceptions;

public sealed class TmcException : Exception
{
    public TmcException() => ErrorCodes = Array.Empty<string>();

    public TmcException(string errorCode)
        : base(errorCode) => ErrorCodes = new[] { errorCode };

    public TmcException(string errorCode, Exception innerException)
        : base(errorCode, innerException) => ErrorCodes = new[] { errorCode };

    public TmcException(params string[] errorCodes)
        : base(errorCodes.FirstOrDefault()) => ErrorCodes = errorCodes;

    public string[] ErrorCodes { get; }
}

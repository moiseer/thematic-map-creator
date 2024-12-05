using System;

namespace ThematicMapCreator.Domain.Exceptions;

public sealed class TmcException : Exception
{
    public TmcException(string errorCode, Exception? innerException = null)
        : base(errorCode, innerException) =>
        ErrorCodes = new[] { errorCode };

    public TmcException(string[] errorCodes, Exception? innerException = null)
        : base(string.Join("; ", errorCodes), innerException) =>
        ErrorCodes = errorCodes;

    public string[] ErrorCodes { get; }
}

﻿using ThematicMapCreator.Domain.Exceptions;

namespace ThematicMapCreator.Domain.Extensions;

public static class ExceptionExtensions
{
    public static string[] GetErrorCodes(this Exception exception) =>
        exception is TmcException tmcException && tmcException.ErrorCodes.Any()
            ? tmcException.ErrorCodes
            : new[] { TmcError.InnerError };
}

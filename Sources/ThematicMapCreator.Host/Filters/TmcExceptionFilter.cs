using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;

namespace ThematicMapCreator.Host.Filters;

public sealed class TmcExceptionFilter : IExceptionFilter
{
    private readonly ILogger logger;

    public TmcExceptionFilter(ILogger<TmcExceptionFilter> logger) => this.logger = logger;

    public void OnException(ExceptionContext context)
    {
        var errorCodes = context.Exception.GetErrorCodes();
        context.Result = new ObjectResult(errorCodes)
        {
            StatusCode = (int?)TmcError.GetHttpStatusCode(errorCodes[0]),
        };
        logger.LogError(context.Exception, "{Message}", string.Join(';', errorCodes));
        context.ExceptionHandled = true;
    }
}

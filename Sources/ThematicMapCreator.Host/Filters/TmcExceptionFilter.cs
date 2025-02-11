using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;

namespace ThematicMapCreator.Host.Filters;

public sealed class TmcExceptionFilter : IExceptionFilter
{
    private readonly ILogger<TmcExceptionFilter> _logger;

    public TmcExceptionFilter(ILogger<TmcExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "{Message}", context.Exception.Message);

        var errorCodes = context.Exception.GetErrorCodes();
        context.Result = new ObjectResult(errorCodes)
        {
            StatusCode = (int?)TmcError.GetHttpStatusCode(errorCodes[0]),
        };
        context.ExceptionHandled = true;
    }
}

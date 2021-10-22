using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;

namespace ThematicMapCreator.Host.Filters
{
    public class TmcExceptionFilter: IExceptionFilter
    {
        private readonly ILogger logger;

        public TmcExceptionFilter(ILogger<TmcExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            string[] errorCodes = context.Exception.GetErrorCodes();
            context.Result = new ObjectResult(errorCodes)
            {
                StatusCode = (int?)TmcError.GetHttpStatusCode(errorCodes.First()),
            };
            logger.LogError(context.Exception, "{Message}", errorCodes.First());
            context.ExceptionHandled = true;
        }
    }
}

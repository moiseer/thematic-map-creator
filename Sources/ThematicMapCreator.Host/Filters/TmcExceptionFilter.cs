using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ThematicMapCreator.Domain.Exceptions;
using ThematicMapCreator.Domain.Extensions;

namespace ThematicMapCreator.Host.Filters
{
    public class TmcExceptionFilter: IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            string[] errorCodes = context.Exception.GetErrorCodes();
            context.Result = new JsonResult(errorCodes)
            {
                StatusCode = (int?)TmcError.GetHttpStatusCode(errorCodes.FirstOrDefault()),
            };
            context.ExceptionHandled = true;
        }
    }
}

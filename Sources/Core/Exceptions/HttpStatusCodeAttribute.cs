using System;
using System.Net;

namespace Core.Exceptions
{
    public class HttpStatusCodeAttribute : Attribute
    {
        public HttpStatusCodeAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}

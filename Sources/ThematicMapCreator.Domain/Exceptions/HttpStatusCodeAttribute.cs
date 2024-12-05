using System;
using System.Net;

namespace ThematicMapCreator.Domain.Exceptions;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class HttpStatusCodeAttribute : Attribute
{
    public HttpStatusCodeAttribute(HttpStatusCode statusCode) => StatusCode = statusCode;

    public HttpStatusCode StatusCode { get; }
}

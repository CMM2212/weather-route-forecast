using System;

namespace Weather.Api.Exceptions;

public class RouteProcessingException : Exception
{
    public RouteProcessingException(string message) : base(message)
    {
    }
    public RouteProcessingException(string message, Exception ex) : base(message, ex)
    {
    }
}
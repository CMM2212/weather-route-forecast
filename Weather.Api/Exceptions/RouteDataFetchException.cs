using System;

namespace Weather.Api.Exceptions;

public class RouteDataFetchException : Exception
{
    public string RequestUri { get; }

    public RouteDataFetchException(string message, string requestUri) : base(message)
    {
        RequestUri = requestUri;
    }

    public RouteDataFetchException(string message, string requestUri, Exception inner) : base(message, inner)
    {
        RequestUri = requestUri;
    }
}
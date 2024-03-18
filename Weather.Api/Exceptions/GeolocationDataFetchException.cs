using System;

namespace Weather.Api.Exceptions;

internal class GeolocationDataFetchException : Exception
{
    public string RequestUri { get; }

    public GeolocationDataFetchException(string message, string requestUri) : base(message)
    {
        RequestUri = requestUri;
    }

    public GeolocationDataFetchException(string message, string requestUri, Exception inner) : base(message, inner)
    {
        RequestUri = requestUri;
    }
}

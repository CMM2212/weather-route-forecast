using System;

namespace Weather.Api.Exceptions;

public class WeatherDataFetchException : Exception
{
    public string RequestUri { get; }

    public WeatherDataFetchException(string message, string requestUri) : base(message)
    {
        RequestUri = requestUri;
    }

    public WeatherDataFetchException(string message, string requestUri, Exception inner) : base(message, inner)
    {
        RequestUri = requestUri;
    }
}
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Weather.Api.Exceptions;
using Weather.Api.Interfaces;
using Weather.Api.Models;

namespace Weather.Api.Services;

public class OpenMeteoClient : IOpenMeteoClient
{
    readonly IHttpClientFactory httpClientFactory;
    readonly ILogger<OpenMeteoClient> logger;

    public OpenMeteoClient(IHttpClientFactory httpClientFactory, ILogger<OpenMeteoClient> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    public async Task<JObject> GetWeatherDataAsync(Waypoint waypoint)
    {
        var httpClient = httpClientFactory.CreateClient(nameof(OpenMeteoClient));

        var requestUri = new UriBuilder(httpClient.BaseAddress)
        {
            Path = "/v1/forecast",
            Query = $"minutely_15=temperature_2m,rain&" +
                    $"latitude={waypoint.Latitude}&longitude={waypoint.Longitude}" +
                    $"&forecast_minutely_15=96"
        }.Uri;
        logger.LogInformation("Requesting weather from Open Meteo: {RequestUri}", requestUri);
        try
        {
            var response = await httpClient.GetAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to fetch weather data from Open Meteo: {ResponseStatusCode}", response.StatusCode);
                throw new WeatherDataFetchException(
                    $"Failed to fetch weather data from Open Meteo: {response.StatusCode}",
                    requestUri.ToString());
            }
            var data = await response.Content.ReadAsStringAsync();
            return JObject.Parse(data);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError("Http request failed: {RequestUri}", requestUri);
            throw new WeatherDataFetchException("HTTP request failed.", requestUri.ToString(), ex);
        }
    }
}

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
            Query = $"minutely_15=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation,rain," +
                    $"snowfall,snowfall_height,sunshine_duration,weather_code,wind_speed_10m," +
                    $"wind_direction_10m,wind_gusts_10m,visibility,lightning_potential,is_day&" +
                    $"latitude={waypoint.Latitude}&longitude={waypoint.Longitude}" +
                    $"&forecast_minutely_15=96" +
                    "&temperature_unit=fahrenheit" +
                    "&precipitation_unit=mm" +
                    "&wind_speed_unit=mph" +
                    "&timezone=America%2FChicago"


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

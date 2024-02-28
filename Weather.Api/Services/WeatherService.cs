using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Weather.Api.Exceptions;
using Weather.Api.Interfaces;
using Weather.Api.Models;

namespace Weather.Api.Services;

public class WeatherService : IWeatherService
{
    readonly IOpenMeteoClient openMeteoClient;
    readonly ILogger<WeatherService> logger;

    public WeatherService(IOpenMeteoClient openMeteoClient, ILogger<WeatherService> logger)
    {
        this.openMeteoClient = openMeteoClient;
        this.logger = logger;
    }

    public async Task<JObject> GetForecasts(List<Waypoint> waypoints)
    {
        var forecasts = new JArray();
        foreach (var waypoint in waypoints)
        {
            try
            {
                logger.LogInformation("Fetching weather data for waypoint: {Waypoint}", waypoint);
                forecasts.Add(await openMeteoClient.GetWeatherDataAsync(waypoint));
            }
            catch (WeatherDataFetchException e)
            {
                logger.LogError(e, "Failed to get weather data for waypoint: {Waypoint}", waypoint);
                throw;
            }
        }
        return new JObject { ["forecasts"] = forecasts };
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Weather.Api.Exceptions;
using Weather.Api.Interfaces;
using Weather.Api.Models;

namespace Weather.Api.Services;

public class AzureMapsClient : IAzureMapsClient
{
    readonly IHttpClientFactory httpClientFactory;
    readonly ILogger<AzureMapsClient> logger;

    public AzureMapsClient(IHttpClientFactory httpClientFactory, ILogger<AzureMapsClient> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    public async Task<JObject> GetRouteDataAsync(List<Waypoint> waypoints)
    {
        var httpClient = httpClientFactory.CreateClient(nameof(AzureMapsClient));

        var requestUri = new UriBuilder(httpClient.BaseAddress)
        {
            Path = "/route/directions/json",
            Query = $"api-version=1.0&query={GenerateQuery(waypoints)}&computeBestOrder=false"
        }.Uri;
        
        logger.LogInformation("Requesting route from Azure Maps: {RequestUri}", requestUri);
        
        try
        {
            var response = await httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to fetch route data from Azure Maps: {ResponseStatusCode}", response.StatusCode);
                throw new RouteDataFetchException(
                    $"Failed to fetch route data from Azure Maps: {response.StatusCode}",
                    requestUri.ToString());
            }

            var data = await response.Content.ReadAsStringAsync();
            logger.LogInformation("Successfully fetched route from Azure Maps");
            return JObject.Parse(data);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError("Http request failed: {RequestUri}", requestUri);
            throw new RouteDataFetchException("HTTP request failed.", requestUri.ToString(), ex);
        }
    }

    static string GenerateQuery(List<Waypoint> waypoints)
    {
        return string.Join(":", waypoints.Select(p => $"{p.Latitude},{p.Longitude}"));
    }
}

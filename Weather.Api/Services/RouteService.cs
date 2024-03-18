using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Weather.Api.Exceptions;
using Weather.Api.Interfaces;
using Weather.Api.Models;
using Weather.Api.Utilities;

namespace Weather.Api.Services;

public class RouteService : IRouteService
{
    readonly IAzureMapsClient azureMapsClient;
    readonly ILogger<RouteService> logger;

    public RouteService(IAzureMapsClient azureMapsClient, ILogger<RouteService> logger)
    {
        this.azureMapsClient = azureMapsClient;
        this.logger = logger;
    }

    public async Task<Route> ProcessRoute(string start, string end)
    {
        logger.LogInformation("Processing route");

        try
        {
            var startWaypoint = await GetWaypointFromLocation(start);
            var endWaypoint = await GetWaypointFromLocation(end);
            var route = new Route(startWaypoint, endWaypoint);
            route.Waypoints = await GetRouteWaypoints(route.Start, route.End);
            route.EvenlySpacedWaypoints = GetEvenlySpacedWaypoints(route.Waypoints);
            route.SampledWaypoints = SampleWaypoints(route.EvenlySpacedWaypoints);
            route.SampledLocations = await GetLocationsFromWaypoints(route.SampledWaypoints);
            route.RouteData = await GetRouteData(route.SampledWaypoints);
            logger.LogInformation("Successfully processed route");
            return route;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process the route");
            throw new RouteProcessingException("Error  processing route.", ex);
        }
    }

    async Task<List<Waypoint>> GetRouteWaypoints(Waypoint start, Waypoint end)
    {
        logger.LogInformation("Fetching route waypoints");
        var waypoints = new List<Waypoint> { start, end};
        var routeData = await azureMapsClient.GetRouteDataAsync(waypoints);
        return ExtractWaypointsFromRouteData(routeData);
    }

    List<Waypoint> GetEvenlySpacedWaypoints(List<Waypoint> waypoints)
    {
        if (waypoints == null || waypoints.Count < 2)
        {
            logger.LogWarning("Not enough waypoints to calculate even spacing");
            return waypoints;
        }
        
        logger.LogInformation("Calculating evenly spaced waypoints");
        var result = new List<Waypoint> { waypoints.First() };
        
        foreach (var waypoint in waypoints.Skip(1).Take(waypoints.Count - 2))
            if (MathUtilities.CalculateDistanceBetweenWaypoints(result.Last(), waypoint) > 1.6)
                result.Add(waypoint);
        
        result.Add(waypoints.Last());
        return result;
    }

    List<Waypoint> SampleWaypoints(List<Waypoint> waypoints, int n  = 20)
    {
        if (waypoints == null || waypoints.Count < n)
        {
            logger.LogWarning("Not enough waypoints to sample; Given: {WaypointsCount}, Required: {N}",
                waypoints?.Count ?? 0, n);
            return waypoints;
        }
        
        logger.LogInformation("Sampling waypoints");
        var result = new List<Waypoint> { waypoints.First() };
        var step = (waypoints.Count - 1) / (n - 1);
        
        for (var i = 1; i < n - 1; i++)
            result.Add(waypoints[i * step]);
        
        result.Add(waypoints.Last());
        return result;
    }

    async Task<JObject> GetRouteData(List<Waypoint> waypoints)
    {
        logger.LogInformation("Fetching route data");
        return await azureMapsClient.GetRouteDataAsync(waypoints);
    }

    List<Waypoint> ExtractWaypointsFromRouteData(JObject routeData)
    {
        if (routeData == null || !routeData["routes"].Any())
        {
            logger.LogError("Received invalid or empty route data");
            throw new RouteProcessingException("Received invalid route data");
        }
        
        try
        {
            return routeData["routes"].First["legs"].First["points"].Select(p => new Waypoint(
                p["latitude"].Value<double>(),
                p["longitude"].Value<double>())
            ).ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to extract waypoints from route data");
            throw new RouteProcessingException("Failed to extract waypoints from route data", e);
        }
    }

    public async Task<Waypoint> GetWaypointFromLocation(string location)
    {
        logger.LogInformation("Fetching waypoint from location: {Location}", location);
        var geolocationData = await azureMapsClient.GetGeolocationDataAsync(location);
        return ExtractWaypointFromGeolocationData(geolocationData);
    }

    private Waypoint ExtractWaypointFromGeolocationData(JObject geolocationData)
    {
        if (geolocationData == null || !geolocationData["results"].Any())
        {
            logger.LogError("Received invalid or empty geolocation data");
            throw new RouteProcessingException("Received invalid geolocation data");
        }

        try
        {
            var location = geolocationData["results"].First["position"];
            return new Waypoint(location["lat"].Value<double>(), location["lon"].Value<double>());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to extract waypoint from geolocation data");
            throw new RouteProcessingException("Failed to extract waypoint from geolocation data", e);
        }
    }

    private async Task<JObject> GetLocationsFromWaypoints(List<Waypoint> waypoints)
    {
        logger.LogInformation("Fetching locations from waypoints");
        var result = new JObject();
        
        foreach (var waypoint in waypoints)
        {
            var query = $"{waypoint.Latitude},{waypoint.Longitude}";
            var geolocationData = await azureMapsClient.GetGeolocationDataAsync(query);
            result[query] = geolocationData;
        }
        
        return result;
    }   
}

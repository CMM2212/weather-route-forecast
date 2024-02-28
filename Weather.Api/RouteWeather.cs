using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Weather.Api.Models;
using Weather.Api.Interfaces;

namespace Weather.Api;

public class RouteWeather
{
    readonly IRouteService routeService;
    readonly IWeatherService weatherService;

    public RouteWeather(IRouteService  routeService, IWeatherService weatherService)
    {
        this.routeService = routeService;
        this.weatherService = weatherService;
    }

    [FunctionName("GetRouteWeather")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "weather/{start}/{end}")] HttpRequest req,
        string start,
        string end,
        ILogger log)
    {
        log.LogInformation("Received request for route from {Start} to {End}", start, end);
        
        if (!TryParseWaypoint(start, out var startWaypoint) || !TryParseWaypoint(end, out var endWaypoint))
        {
            return new BadRequestObjectResult("Invalid start or end waypoint");
        }
        
        try
        {
            var route = await routeService.ProcessRoute(startWaypoint, endWaypoint);
            var forecasts = await weatherService.GetForecasts(route.SampledWaypoints);

            var result = new JObject
            {
                ["route"] = route.RouteData,
                ["weather"] = forecasts
            };

            return new OkObjectResult(result.ToString());

        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to process route and weather");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
    
    static bool TryParseWaypoint(string input, out Waypoint waypoint)
    {
        waypoint = null;
        var parts = input.Split(',');
        
        if (parts.Length != 2)
            return false;
        
        if (!double.TryParse(parts[0], out var latitude) || !double.TryParse(parts[1], out var longitude))
            return false;
        
        waypoint = new Waypoint(latitude, longitude);
        return true;
    }
}

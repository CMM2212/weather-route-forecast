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
using Weather.Api.Services;

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
        try
        {
            var route = await routeService.ProcessRoute(start, end);
            var forecasts = await weatherService.GetForecasts(route.SampledWaypoints);

            var result = new JObject
            {
                ["route"] = route.RouteData,
                ["locations"] = route.SampledLocations,
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
}

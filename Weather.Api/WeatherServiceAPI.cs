using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Weather.Api;

public static class WeatherServiceAPI
{

    [FunctionName("GetRouteWeather")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        List<Waypoint> waypoints = new()
        {
            new Waypoint { Latitude = 46.877186, Longitude = -96.789803 },
            new Waypoint { Latitude = 45.019009, Longitude = -93.586082 }
        };
        var route = await RouteHelper.GetRouteJSON(waypoints);
        var allRouteWaypoints = RouteHelper.GetWaypointsFromJSON(route);
        var sampledWaypoints = RouteHelper.SampleRoute(allRouteWaypoints, 1.6);
        var estimatedWaypoints = RouteHelper.GetNWaypoints(allRouteWaypoints);
        var stringResult = JsonConvert.SerializeObject(estimatedWaypoints);
        var detailedRoute = await RouteHelper.GetRouteJSON(estimatedWaypoints);
        var weather = await WeatherHelper.GetWeather(estimatedWaypoints);

        var result = new JObject
        {
            ["detailedRoute"] = detailedRoute,
            ["weather"] = JArray.FromObject(weather)
        };


        return new OkObjectResult(result.ToString());

        string name = req.Query["name"];

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name = name ?? data?.name;

        string responseMessage = string.IsNullOrEmpty(name)
            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            : $"Hello, {name}. This HTTP triggered function executed successfully.";

        return new OkObjectResult(responseMessage);
    }
}

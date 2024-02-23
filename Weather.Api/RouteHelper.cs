using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Api;

public static class RouteHelper
{
    static readonly HttpClient client = new HttpClient();

    public static async Task<JObject> GetRouteJSON(List<Waypoint> waypoints)
    {
        // Test locally
        if (waypoints.Count == 2)
            return JObject.Parse(await File.ReadAllTextAsync("route.json"));
        else
            return JObject.Parse(await File.ReadAllTextAsync("detailedRoute.json"));
        // Actual implementation:

        var baseUrl = "https://atlas.microsoft.com/route/directions/json";
        var queryParameters = new Dictionary<string, string>
        {
            { "api-version", "1.0" },
            { "query", string.Join(":", waypoints.Select(w => $"{w.Latitude},{w.Longitude}")) },
            { "subscription-key", Environment.GetEnvironmentVariable("AzureMapsSubscriptionKey") },
            {"computeBestOrder", "false"}
        };
        var queryString = await new FormUrlEncodedContent(queryParameters).ReadAsStringAsync();
        var requestUri = $"{baseUrl}?{queryString}";
        var response = await client.GetAsync(requestUri);
        var data = await response.Content.ReadAsStringAsync();
        return JObject.Parse(data);
    }

    public static List<Waypoint> GetWaypointsFromJSON(JObject route)
    {
        var waypoints = route["routes"].First["legs"].First["points"].Select(p => new Waypoint
        {
            Latitude = p["latitude"].Value<double>(),
            Longitude = p["longitude"].Value<double>()
        }).ToList();
        return waypoints;
    }

    public static List<Waypoint> SampleRoute(List<Waypoint> waypoints, double minDistance = 1.6)
    {
        var result = new List<Waypoint> { waypoints.First() };
        foreach (var waypoint in waypoints.Skip(1).Take(waypoints.Count - 2))
            if (CalculateDistanceBetweenWaypoints(result.Last(), waypoint) > minDistance)
                result.Add(waypoint);
        result.Add(waypoints.Last());
        return result;
    }   


    public static double CalculateDistanceBetweenWaypoints(Waypoint a, Waypoint b)
    {
        const double earthRadius = 6371;

        var latitudeInRadiansA = DegreesToRadians(a.Latitude);
        var latitudeInRadiansB = DegreesToRadians(b.Latitude);
        var longitudeInRadiansA = DegreesToRadians(a.Longitude);
        var longitudeInRadiansB = DegreesToRadians(b.Longitude);

        var deltaLatitude = latitudeInRadiansB - latitudeInRadiansA;
        var deltaLongitude = longitudeInRadiansB - longitudeInRadiansA;

        var calculation = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                          Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2) *
                          Math.Cos(latitudeInRadiansA) * Math.Cos(latitudeInRadiansB);
        var c = 2 * Math.Asin(Math.Sqrt(calculation));
        return earthRadius * c;
    }

    public static double DegreesToRadians(double degrees)
    {
        return Math.PI * degrees / 180.0;
    }

    public static List<Waypoint> GetNWaypoints(List<Waypoint> waypoints, int n=20)
    {
        var result = new List<Waypoint> { waypoints.First() };
        var step = (waypoints.Count - 1) / (n - 1);
        for (var i = 1; i < n - 1; i++)
            result.Add(waypoints[i * step]);
        result.Add(waypoints.Last());
        return result;
    }

}

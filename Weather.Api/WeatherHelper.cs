using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Api;

public class WeatherHelper
{
    static readonly HttpClient client = new();

    public static async Task<List<JObject>> GetWeather(List<Waypoint> waypoints)
    {
        /*// Cached testing:
        var jsonArray = JArray.Parse(await File.ReadAllTextAsync("weather.json"));
        var results = jsonArray.Select(j => (JObject)j).ToList();
        return results;*/

        // Actual implementation:
        var forecasts = new List<JObject>();
        foreach (var waypoint in waypoints)
        {
            var forecast = await GetWeatherJSON(waypoint);
            forecasts.Add(forecast);
        }
        return forecasts;
    }

    public static async Task<JObject> GetWeatherJSON(Waypoint waypoint)
    {
        var baseUrl = "https://api.open-meteo.com/v1/forecast";
        var queryParameters = new Dictionary<string, string>
        {
            { "latitude", waypoint.Latitude.ToString() },
            { "longitude", waypoint.Longitude.ToString() },
            {"minutely_15", "temperature_2m,rain" },
            { "forecast_minutely_15", "96" }
        };
        var queryString = await new FormUrlEncodedContent(queryParameters).ReadAsStringAsync();
        var requestUri = $"{baseUrl}?{queryString}";
        var response = await client.GetAsync(requestUri);
        var data = await response.Content.ReadAsStringAsync();
        return JObject.Parse(data);
    }
}

using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Weather.Web.Models;
using Weather.Web.Models.API;
using static System.Net.WebRequestMethods;

namespace Weather.Web.Services;

public class WeatherForecastService
{
    private readonly HttpClient httpClient;

    public WeatherForecastService(HttpClient httpClient)    
    {
        this.httpClient = httpClient;
    }

    public async Task<Route> GetWeatherForecastAsync(string startLocation, string endLocation)
    {
        Console.WriteLine($"Getting weather forecast for {startLocation} to {endLocation}. at api/weather/{startLocation}/{endLocation}");
        var routeWeatherJson = await httpClient.GetStringAsync($"api/weather/{startLocation}/{endLocation}");
        //var routeWeatherJson = await httpClient.GetStringAsync("https://localhost:7165/nashvilleToJacksonResponse.json");
        //var routeWeatherJson = await httpClient.GetStringAsync("https://localhost:7165/fargoToMinneapolisResponse.json");
        var route = ParseRouteFromJson(routeWeatherJson);
        return route;
    }

    public Route ParseRouteFromJson(string routeJson)
    {
        using var doc = JsonDocument.Parse(routeJson);
        var root = doc.RootElement;

        // root["route"]["routes"][0]
        var routeData = root.GetProperty("route").GetProperty("routes").EnumerateArray().First();

        // root["route"]["routes"][0]["legs"]
        var legs = routeData.GetProperty("legs").EnumerateArray().ToArray();
        var locations = root.GetProperty("locations").EnumerateObject().Select(pair => pair.Value).ToArray();
        var weather = root.GetProperty("weather").GetProperty("forecasts").EnumerateArray().ToArray();
        var routeStartTime = legs.First().GetProperty("summary").GetProperty("departureTime").GetDateTime();

        var route = new Route()
        {
            Legs = []
        };

        // Parse all but final leg
        for (int i = 0; i < legs.Length; i++)
        {
            var leg = ParseLegFromJson(legs[i], locations[i], weather[i], routeStartTime);
            route.Legs.Add(leg);
        }


        // Parse final leg
        var finalLeg = CreateFinalLeg(legs.Last(), weather.Last(), locations.Last(), routeStartTime);
        route.Legs.Add(finalLeg);

        route.Start = route.Legs.First().Start;
        route.End = finalLeg.Start;

        return route;
    }

    public Leg ParseLegFromJson(JsonElement legJson, JsonElement locationJson, JsonElement weatherJson, DateTime routeStartTime)
    {
        var startTime = legJson.GetProperty("summary").GetProperty("departureTime").GetDateTime();
        var startLocation = legJson.GetProperty("points").EnumerateArray().First();
        var locationName = ParseLocationNameFromJson(locationJson);
        var location = new Location()
        {
            Latitude = startLocation.GetProperty("latitude").GetDouble(),
            Longitude = startLocation.GetProperty("longitude").GetDouble(),
            Name = locationName
        };

        var forecasts = ParseForecastsFromJson(weatherJson, routeStartTime, location);

        var leg = new Leg()
        {
            Time = startTime,
            Start = location,
            Forecasts = forecasts
        };
        return leg;
    }


    private string ParseLocationNameFromJson(JsonElement locationJson)
    {
        var bestMatchLocation = locationJson.GetProperty("results").EnumerateArray().First();
        var address = bestMatchLocation.GetProperty("address");
        if (address.TryGetProperty("localName", out var localName) && !string.IsNullOrWhiteSpace(localName.GetString()))
            return localName.GetString();
        if (address.TryGetProperty("neighbourhood", out var neighbourhood) && !string.IsNullOrWhiteSpace(neighbourhood.GetString()))
            return neighbourhood.GetString();
        if (address.TryGetProperty("municipality", out var municipality) && !string.IsNullOrWhiteSpace(municipality.GetString()))
            return municipality.GetString();
        return address.GetProperty("freeformAddress").GetString();

    }

    private List<WeatherForecast> ParseForecastsFromJson(JsonElement weatherJson, DateTime startTime, Location location)
    {
        var forecasts = new List<WeatherForecast>();
        var minutely15Forecasts = weatherJson.GetProperty("minutely_15");
        var temperature = minutely15Forecasts.GetProperty("temperature_2m").EnumerateArray().ToArray();
        var humidity = minutely15Forecasts.GetProperty("relative_humidity_2m").EnumerateArray().ToArray();
        var apparentTemperature = minutely15Forecasts.GetProperty("apparent_temperature").EnumerateArray().ToArray();
        var precipitation = minutely15Forecasts.GetProperty("precipitation").EnumerateArray().ToArray();
        var rain = minutely15Forecasts.GetProperty("rain").EnumerateArray().ToArray();
        var snowfall = minutely15Forecasts.GetProperty("snowfall").EnumerateArray().ToArray();
        var snowFallHeight = minutely15Forecasts.GetProperty("snowfall_height").EnumerateArray().ToArray();
        var sunshineDuration = minutely15Forecasts.GetProperty("sunshine_duration").EnumerateArray().ToArray();
        var weatherCode = minutely15Forecasts.GetProperty("weather_code").EnumerateArray().ToArray();
        var windSpeed = minutely15Forecasts.GetProperty("wind_speed_10m").EnumerateArray().ToArray();
        var windDirection = minutely15Forecasts.GetProperty("wind_direction_10m").EnumerateArray().ToArray();
        var windGusts = minutely15Forecasts.GetProperty("wind_gusts_10m").EnumerateArray().ToArray();
        var visibility = minutely15Forecasts.GetProperty("visibility").EnumerateArray().ToArray();
        var lightningPotential = minutely15Forecasts.GetProperty("lightning_potential").EnumerateArray().ToArray();
        var isDay = minutely15Forecasts.GetProperty("is_day").EnumerateArray().ToArray();

        for (int i = 0; i < temperature.Count(); i++)
        {
            var time = startTime.AddMinutes(i * 15);
            forecasts.Add(new WeatherForecast()
            {
                Time = time,
                Location = location,
                Temperature = temperature[i].GetDouble(),
                Humidity = humidity[i].GetDouble(),
                ApparentTemperature = apparentTemperature[i].GetDouble(),
                Precipitation = precipitation[i].GetDouble(),
                Rain = rain[i].GetDouble(),
                Snow = snowfall[i].GetDouble(),
                SnowFallHeight = ParseDouble(snowFallHeight[i]),
                SunshineDuration = sunshineDuration[i].GetDouble(),
                WeatherCode = weatherCode[i].GetInt32(),
                WindSpeed = windSpeed[i].GetDouble(),
                WindDirection = windDirection[i].GetInt32(),
                WindGust = windGusts[i].GetDouble(),
                Visibility = visibility[i].GetDouble(),
                Lightning = ParseDouble(snowFallHeight[i]),
                IsDay = isDay[i].GetInt32() == 1

        });
        }
        return forecasts;
    }

    private double ParseDouble(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Number ? element.GetDouble() : 0;
    }

    private Leg CreateFinalLeg(JsonElement lastLegJson, JsonElement finalWeatherJson, JsonElement finalLocationJson, DateTime routeStartTime)
    {
        var startTime = lastLegJson.GetProperty("summary").GetProperty("arrivalTime").GetDateTime();

        var startLocation = lastLegJson.GetProperty("points").EnumerateArray().Last();
        var locationName = ParseLocationNameFromJson(finalLocationJson);
        var location = new Location()
        {
            Latitude = startLocation.GetProperty("latitude").GetDouble(),
            Longitude = startLocation.GetProperty("longitude").GetDouble(),
            Name = locationName
        };

        var forecasts = ParseForecastsFromJson(finalWeatherJson, routeStartTime, location);

        var leg = new Leg()
        {
            Time = startTime,
            Start = location,
            Forecasts = forecasts
        };
        return leg;
    }

    public List<WeatherForecast> GetWeatherForecastByTimeAndInterval(Route route, int offsetMinutes, int intervalMinutes)
    {
        var result = new List<WeatherForecast>();
        var legs = GetLegsByTimeInterval(route, intervalMinutes);
        for (int i = 0; i < legs.Count; i++)
        {
            var leg = legs[i];
            int index = (i * intervalMinutes + offsetMinutes) / 15;
            var forecast = leg.Forecasts[index];
            result.Add(forecast);
        }

        return result;
    }

    public List<Leg> GetLegsByTimeInterval(Route route, int intervalMinutes)
    {
        var legs = new List<Leg>();
        var currentTime = route.Legs.First().Time;
        var endTime = route.Legs.Last().Time;

        //Console.WriteLine($"Current time: {currentTime}, end time: {endTime}, leg count: {legs.Count}");
        while (currentTime < endTime)
        {
            var currentLeg = route.Legs.First(leg => leg.Time >= currentTime);
            legs.Add(currentLeg);
            currentTime = currentTime.AddMinutes(intervalMinutes);
            //Console.WriteLine($"Leg added: {currentLeg.Time}, current time: {currentTime}, end time: {endTime}, leg count: {legs.Count}");
        }
        legs.Add(route.Legs.Last());
        return legs;
    }
}

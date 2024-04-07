using System.Net;
using System.Text.Json;

namespace Weather.Web.Models;

public class RouteWeather
{
    public string StartLocation { get; set; }
    public string EndLocation { get; set; }
    public List<RouteLeg> RouteLegs { get; set; }

    public static RouteWeather FromJson(string json)
    {
        var data = JsonDocument.Parse(json).RootElement;
        var routeData = data.GetProperty("route").GetProperty("routes");
        var summary = routeData.EnumerateArray().First().GetProperty("summary");
        var legs = routeData.EnumerateArray().First().GetProperty("legs").EnumerateArray().ToList();
        var locations = data.GetProperty("locations").EnumerateObject().Select(x => x.Value).ToList();
        var weather = data.GetProperty("weather").GetProperty("forecasts");

        var routeLegs = new List<RouteLeg>();
        for (int i = 0; i < locations.Count; i++)
        {
            var location = locations[i].GetProperty("results").EnumerateArray().First();
            var address = location.GetProperty("address");
            var forecast = weather[i].GetProperty("minutely_15");

            var locationDetails = new LocationDetails
            {
                LocalName = address.GetProperty("freeformAddress").GetString(),
            };

            var weatherForecast = new WeatherForecast
            {
                Rain = forecast.GetProperty("rain").EnumerateArray().Select(r => r.GetDouble()).ToList(),
                Temperature = forecast.GetProperty("temperature_2m").EnumerateArray().Select(t => t.GetDouble()).ToList()
            };

            double latitude, longitude;
            DateTime time;
            if (i == locations.Count - 1)
            {
                var leg = legs[i - 1];
                time = leg.GetProperty("summary").GetProperty("arrivalTime").GetDateTime();
                latitude = leg.GetProperty("points").EnumerateArray().Last().GetProperty("latitude").GetDouble();
                longitude = leg.GetProperty("points").EnumerateArray().Last().GetProperty("longitude").GetDouble();
            }
            else
            {
                var leg = legs[i];
                time = leg.GetProperty("summary").GetProperty("departureTime").GetDateTime();
                latitude = leg.GetProperty("points").EnumerateArray().First().GetProperty("latitude").GetDouble();
                longitude = leg.GetProperty("points").EnumerateArray().First().GetProperty("longitude").GetDouble();
            }
            
            var routeLeg = new RouteLeg
            {
                Address = locationDetails,
                Time = time,
                Latitude = latitude,
                Longitude = longitude,
                Forecast = weatherForecast
            };
            Console.WriteLine(routeLeg);
            routeLegs.Add(routeLeg);
        }   

        var routeWeather = new RouteWeather
        {
            StartLocation = routeLegs.First().Address.LocalName,
            EndLocation = routeLegs.Last().Address.LocalName,
            RouteLegs = routeLegs
        };
        return routeWeather;
    }
}

using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Weather.Web.Models;
using static System.Net.WebRequestMethods;

namespace Weather.Web.Services;

public class WeatherForecastService
{
    private readonly HttpClient httpClient;

    public WeatherForecastService(HttpClient httpClient)    
    {
        this.httpClient = httpClient;
    }

    public async Task<RouteWeather> GetWeatherForecastAsync(string startLocation, string endLocation)
    {
        // Example: Fetch route weather data from your API
        //var routeWeatherJson = await httpClient.GetStringAsync($"api/routeWeather?start={startLocation}&end={endLocation}");
        var routeWeatherJson = await httpClient.GetStringAsync("https://localhost:7165/fargoToMinneapolisResponse.json");
        var routeWeather = RouteWeather.FromJson(routeWeatherJson);
        return routeWeather;
    }

    public List<WeatherForecast> Get15MinutelyWeatherForecast(RouteWeather routeWeather, int minutesOffSet)
    {
        var weatherForecasts = new List<WeatherForecast>();
        var startTime = routeWeather.RouteLegs.First().Time.AddMinutes(minutesOffSet);
        var endTime = routeWeather.RouteLegs.Last().Time.AddMinutes(minutesOffSet);

        var time = startTime;

        while (time <= endTime)
        {
            var nextLeg = routeWeather.RouteLegs.FirstOrDefault(leg => leg.Time >= time) ?? routeWeather.RouteLegs.Last();
            var forecast = GetForecastForTime(nextLeg, time);
            weatherForecasts.Add(forecast);
            time = time.AddMinutes(15);
        }
    return weatherForecasts;
    }

    private WeatherForecast GetForecastForTime(RouteLeg routeLeg, DateTime time)
    {
        var forecast = new WeatherForecast();
        var leg = routeLeg;
        var forecastTime = time - leg.Time;
        var forecastIndex = (int)forecastTime.TotalMinutes / 15;
        forecast.Rain = leg.Address.LocalName;
        forecast.Temperature = leg.Address.LocalName;
        return forecast;
    }

}

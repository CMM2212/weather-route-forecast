namespace Weather.Web.Models.API;

public class Leg
{
    public Location Start { get; set; }
    public DateTime Time { get; set; }
    public List<WeatherForecast> Forecasts { get; set; }
}

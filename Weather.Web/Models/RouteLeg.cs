namespace Weather.Web.Models;

public class RouteLeg
{
    public LocationDetails Address { get; set; }
    public DateTime Time { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public WeatherForecast Forecast { get; set; }
}

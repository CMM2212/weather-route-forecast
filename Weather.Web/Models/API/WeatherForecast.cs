namespace Weather.Web.Models.API;

public class WeatherForecast
{
    public DateTime Time { get; set; }
    public Location Location { get; set; }
    public double Temperature { get; set; }
    public double Precipitation { get; set; }
}

namespace Weather.Web.Models;

public class WeatherForecast
{
    public string Location { get; set; }
    public DateTime Time { get; set; }
    public List<double> Rain { get; set; }
    public List<double> Temperature { get; set; }
}

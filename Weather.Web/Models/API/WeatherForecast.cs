namespace Weather.Web.Models.API;

public class WeatherForecast
{
    public DateTime Time { get; set; }
    public Location Location { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double ApparentTemperature { get; set; }
    public double Precipitation { get; set; }
    public double Rain { get; set; }
    public double Snow { get; set; }
    public double SnowFallHeight { get; set; }
    public double SunshineDuration { get; set; }
    public int WeatherCode { get; set; }
    public double WindSpeed { get; set; }
    public int WindDirection { get; set; }
    public double WindGust { get; set; }
    public double Visibility { get; set; }
    public double Lightning { get; set; }
    public bool IsDay { get; set; }

}

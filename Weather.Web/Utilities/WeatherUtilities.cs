namespace Weather.Web.Utilities;

public static class WeatherUtilities
{
    public static string GetTemperatureColor(double temperature)
    {

        double minTemp = 32;
        double maxTemp = 100;

        double normalizedTemp = (temperature - minTemp) / (maxTemp - minTemp);

        int r = (int)(255 * normalizedTemp);
        int b = 255 - r;
        int g = 0;
        r = Math.Clamp(r, 0, 255);
        b = Math.Clamp(b, 0, 255);

        return $"background-color: rgb({r},{g},{b});";
    }

    public static string GetPrecipitationColor(double precipitation)
    {
        double minPrecip = 0;
        double maxPrecip = 0.005;

        double normalizedPrecip = (precipitation - minPrecip) / (maxPrecip - minPrecip);

        int r = 40;
        int b = (int)(255 * normalizedPrecip - 40);
        int g = 255 - b - 100;
        b = Math.Clamp(b, 0, 255);
        g = Math.Clamp(g, 0, 255);

        return $"background-color: rgb({r},{g},{b});";
    }

    public static string GetWeatherIcon(int weatherCode, bool isDay)
    {
        return weatherCode switch
        {
            0 => isDay ? "wi-day-sunny" : "wi-night-clear",
            1 => isDay ? "wi-day-cloudy" : "wi-night-partly-cloudy",
            2 => isDay ? "wi-cloud" : "wi-night-alt-cloudy",
            3 => "wi-cloudy",
            45 => isDay ? "wi-day-fog" : "wi-night-fog",
            48 => isDay ? "wi-day-fog" : "wi-night-fog",
            51 or 53 or 55 or 56 or 57 => isDay ? "wi-day-sprinkle" : "wi-night-alt-sprinkle",
            61 or 63 or 65 => isDay ? "wi-day-rain" : "wi-night-alt-rain",
            66 or 67 => isDay ? "wi-day-rain-mix" : "wi-night-alt-rain-mix",
            71 or 73 or 75 or 77 => isDay ? "wi-day-snow" : "wi-night-alt-snow",
            80 or 81 or 82 => isDay ? "wi-day-showers" : "wi-night-alt-showers",
            85 or 86 => "wi-snow-wind",
            95 => isDay ? "wi-day-thunderstorm" : "wi-night-alt-storm-thunderstorm",
            96 or 99 => "wi-storm-showers",
            _ => "wi-na"
        };
    }
}

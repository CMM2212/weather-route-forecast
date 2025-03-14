﻿namespace Weather.Web.Utilities;

public static class WeatherUtilities
{
    public static string GetTemperatureColor(double temperature)
    {

        double minTemp = 10;
        double maxTemp = 100;

        return GetColorGradient(temperature, minTemp, maxTemp, (0, 50, 255), (255, 20, 20), false);
    }

    public static string GetPrecipitationColor(double precipitation)
    {

        return GetColorGradient(precipitation, 0, 3, (247, 247, 247), (55, 170, 173), true);
    }

    public static string GetWindSpeedColor(double windSpeed)
    {
        return GetColorGradient(windSpeed, 0, 30, (247, 247, 247), (55, 170, 173), true);
    }


    public static string GetColorGradient(double value, double minValue, double maxValue, (int r, int g, int b) startColor, (int r, int g, int b) endColor, bool sqrt)
    {
        double normalizedValue = Math.Clamp((value - minValue) / (maxValue - minValue), 0, 1);

        double factor;
        if (sqrt)
            factor = Math.Sqrt(normalizedValue);
        else
            factor = normalizedValue;


        int r = (int)(startColor.r + factor * (endColor.r - startColor.r));
        int g = (int)(startColor.g + factor * (endColor.g - startColor.g));
        int b = (int)(startColor.b + factor * (endColor.b - startColor.b));

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

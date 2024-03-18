using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Weather.Api.Interfaces;
using Weather.Api.Services;

[assembly: FunctionsStartup(typeof(Weather.Api.Startup))]

namespace Weather.Api;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = builder.GetContext().Configuration;

        builder.Services.AddHttpClient(nameof(AzureMapsClient), client =>
        {
            client.BaseAddress = new Uri("https://atlas.microsoft.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("subscription-key", 
                Environment.GetEnvironmentVariable("AzureMapsSubscriptionKey"));
        });

        builder.Services.AddHttpClient(nameof(OpenMeteoClient), client =>
        {
            client.BaseAddress = new Uri("https://api.open-meteo.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddSingleton<IAzureMapsClient, AzureMapsClient>();
        builder.Services.AddSingleton<IRouteService, RouteService>();
        builder.Services.AddSingleton<IWeatherService, WeatherService>();
        builder.Services.AddSingleton<IOpenMeteoClient, OpenMeteoClient>();
    }
}
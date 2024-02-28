using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Weather.Api.Models;

namespace Weather.Api.Interfaces;

public interface IWeatherService
{
    Task<JObject> GetForecasts(List<Waypoint> waypoints);
}

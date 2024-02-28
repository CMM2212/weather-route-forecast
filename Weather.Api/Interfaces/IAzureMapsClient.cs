using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Weather.Api.Models;

namespace Weather.Api.Interfaces;

public interface IAzureMapsClient
{
    Task<JObject> GetRouteDataAsync(List<Waypoint> waypoints);
}

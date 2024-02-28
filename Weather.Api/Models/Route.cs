using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Weather.Api.Models;

public class Route
{
    public Route(Waypoint start, Waypoint end)
    {
        Start = start;
        End = end;
    }

    public Waypoint Start { get; }
    public Waypoint End { get; }
    public List<Waypoint> Waypoints { get; set; }
    public List<Waypoint> EvenlySpacedWaypoints { get; set; }
    public List<Waypoint> SampledWaypoints { get; set; }
    public JObject RouteData { get; set; }
}

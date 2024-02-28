using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Api.Models;

public class Waypoint
{
    public Waypoint(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public override string ToString()
    {
        return $"{Latitude},{Longitude}";
    }

    public double Latitude { get; init; }
    public double Longitude { get; init; }
}

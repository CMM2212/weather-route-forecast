using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Api.Models;

namespace Weather.Api.Utilities;

public static class MathUtilities
{
    public static double CalculateDistanceBetweenWaypoints(Waypoint a, Waypoint b)
    {
        const double earthRadius = 6371;

        var latitudeInRadiansA = DegreesToRadians(a.Latitude);
        var latitudeInRadiansB = DegreesToRadians(b.Latitude);
        var longitudeInRadiansA = DegreesToRadians(a.Longitude);
        var longitudeInRadiansB = DegreesToRadians(b.Longitude);

        var deltaLatitude = latitudeInRadiansB - latitudeInRadiansA;
        var deltaLongitude = longitudeInRadiansB - longitudeInRadiansA;

        var calculation = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                          Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2) *
                          Math.Cos(latitudeInRadiansA) * Math.Cos(latitudeInRadiansB);
        var c = 2 * Math.Asin(Math.Sqrt(calculation));
        return earthRadius * c;
    }

    static double DegreesToRadians(double degrees)
    {
        return Math.PI * degrees / 180.0;
    }
}

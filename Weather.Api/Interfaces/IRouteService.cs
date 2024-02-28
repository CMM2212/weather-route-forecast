using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Api.Models;

namespace Weather.Api.Interfaces;

public interface IRouteService
{
    Task<Route> ProcessRoute(Waypoint start, Waypoint end);
}

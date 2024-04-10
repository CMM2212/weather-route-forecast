namespace Weather.Web.Models.API;

public class Route
{
    public Location Start { get; set; }
    public Location End { get; set; }
    public List<Leg> Legs { get; set; }
}

namespace Weather.Web.Models;

public class UserRouteModel
{
    public UserRouteModel()
    {
        StartLatitude = 46.877186;
        StartLongitude = -96.789803;
        EndLatitude = 45.019009;
        EndLongitude = -93.586082;
    }

    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public double EndLatitude { get; set; }
    public double EndLongitude { get; set; }
}

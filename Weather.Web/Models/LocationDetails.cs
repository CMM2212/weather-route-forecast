namespace Weather.Web.Models;

public class LocationDetails
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string StateCode { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string LocalName { get; set; }
    public string FreeFormAddress { get; set; }
}

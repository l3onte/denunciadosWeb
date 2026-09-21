namespace denunciadosWeb.Models;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Municipality { get; set; } = String.Empty;
    public int DistrictNumber { get; set; }
}
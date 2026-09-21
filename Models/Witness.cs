namespace denunciadosWeb.Models;

public class Witness
{
    public int Id { get; set; }
    public int ComplaintId { get; set; }
    public string Name { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Identification { get; set; } = String.Empty;
    public string Sex { get; set; } = String.Empty;
    public int Age { get; set; }

    public Complaint? Complaint { get; set; }
}
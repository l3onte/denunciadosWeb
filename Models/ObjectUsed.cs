namespace denunciadosWeb.Models;

public class ObjectUsed
{
    public int Id { get; set; }
    public int ComplaintId { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public int Amount { get; set; }

    public Complaint? Complaint { get; set; }
}
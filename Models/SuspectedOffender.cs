namespace denunciadosWeb.Models;

public class SuspectedOffender
{
    public int Id { get; set; }
    public int ComplaintId { get; set; }
    public string Name { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Alias { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string SkinColor { get; set; } = String.Empty;
    public Decimal ApproximateHeight { get; set; }
    public string Hair { get; set; } = String.Empty;
    public string Build { get; set; } = String.Empty;
    public string Sex { get; set; } = String.Empty;
    public string Tatoos { get; set; } = String.Empty;
    public string Scars { get; set; } = String.Empty;

    public Complaint? Complaint { get; set; }
}
namespace denunciadosWeb.Models;

public class ComplaintFilterViewModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }


    public int? CrimeTypeId { get; set; }
    public int? LocationTypeId { get; set; }
    public int? ComplaintStatusId { get; set; }

    public string? Municipality { get; set; }
    public string? Location { get; set; }
    public int? District { get; set; }
}
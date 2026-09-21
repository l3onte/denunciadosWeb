namespace denunciadosWeb.Models;

public class Complaint
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LocationId { get; set; }
    public int LocationTypeId { get; set; }
    public int TypeOfCrimeId { get; set; }
    public int ComplaintStatusId { get; set; }
    public DateTime DateAndtimeOfOccurrence { get; set; }
    public string Direction { get; set; } = String.Empty;
    public string Synthesis { get; set; } = String.Empty;
    public int MasculineQuantities { get; set; }
    public int FamaleQuantities { get; set; }
    public int UnknownQuantities { get; set; }
    public DateTime DateAndTimeOfRegistration { get; set; }

    public User? User { get; set; }
    public Location? Location { get; set; }
    public LocationType? LocationType { get; set; }
    public TypeOfCrime? TypeOfCrime { get; set; }
    public ComplaintStatus? ComplaintStatus { get; set; }
}
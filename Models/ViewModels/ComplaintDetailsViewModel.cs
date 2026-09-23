namespace denunciadosWeb.Models;

public class ComplaintDetailsViewModel
{
    public int Id { get; set; }

    public DateTime DateAndTimeOfOccurrence { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string? Direction { get; set; }

    public string Synthesis { get; set; } = string.Empty;

    public int MasculineQuantities { get; set; }

    public int FemaleQuantities { get; set; }

    public int UnknownQuantities { get; set; }

    public UserDetailViewModel User { get; set; } = new();

    public LocationDetailViewModel Location { get; set; } = new();

    public LocationTypeDetailViewModel LocationType { get; set; } = new();

    public CrimeTypeDetailViewModel CrimeType { get; set; } = new();

    public ComplaintStatusDetailViewModel ComplaintStatus { get; set; } = new();

    public List<VictimDetailViewModel> Victims { get; set; } = new();

    public List<WitnessDetailViewModel> Witnesses { get; set; } = new();

    public List<PresumedAuthorDetailViewModel> PresumedAuthors { get; set; } = new();

    public List<AffectedObjectDetailViewModel> AffectedObjects { get; set; } = new();

    public List<UsedObjectDetailViewModel> UsedObjects { get; set; } = new();
}

public class UserDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}

public class LocationDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Municipality { get; set; } = string.Empty;

    public int DistrictNumber { get; set; }
}

public class LocationTypeDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class CrimeTypeDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class ComplaintStatusDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

public class VictimDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Identification { get; set; }

    public string? Sex { get; set; }

    public int? Age { get; set; }
}

public class WitnessDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Identification { get; set; }

    public string? Sex { get; set; }

    public int? Age { get; set; }
}

public class PresumedAuthorDetailViewModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string? Alias { get; set; }

    public string? Description { get; set; }

    public string? SkinColor { get; set; }

    public decimal? ApproximateHeight { get; set; }

    public string? Hair { get; set; }

    public string? Build { get; set; }

    public string? Sex { get; set; }

    public string? Tattoos { get; set; }

    public string? Scars { get; set; }
}

public class AffectedObjectDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Quantity { get; set; }
}

public class UsedObjectDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Quantity { get; set; }
}
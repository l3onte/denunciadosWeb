using System.ComponentModel.DataAnnotations;

namespace denunciadosWeb.Models;

public class CreateComplaintViewModel
{
    [Required(ErrorMessage = "La fecha y hora del hecho son obligatorias.")]
    public DateTime? DateAndTimeOfOccurrence { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una localidad.")]
    public int? LocationId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de lugar.")]
    public int? LocationTypeId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de delito.")]
    public int? CrimeTypeId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    public int? ComplaintStatusId { get; set; }

    public string? Direction { get; set; }

    [Required(ErrorMessage = "La síntesis del hecho es obligatoria.")]
    [StringLength(500)]
    public string Synthesis { get; set; } = string.Empty;

    public List<VictimInputViewModel> Victims { get; set; } = new();

    public List<WitnessInputViewModel> Witnesses { get; set; } = new();

    public List<PresumedAuthorInputViewModel> PresumedAuthors { get; set; } = new();

    public List<AffectedObjectInputViewModel> AffectedObjects { get; set; } = new();

    public List<UsedObjectInputViewModel> UsedObjects { get; set; } = new();
}

public class VictimInputViewModel
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Identification { get; set; }
    public string? Sex { get; set; }
    public int? Age { get; set; }
}

public class WitnessInputViewModel
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Identification { get; set; }
    public string? Sex { get; set; }
    public int? Age { get; set; }
}

public class PresumedAuthorInputViewModel
{
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

public class AffectedObjectInputViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UsedObjectInputViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
}
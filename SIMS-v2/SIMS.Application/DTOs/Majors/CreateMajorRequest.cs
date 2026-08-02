using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Majors;

/// <summary>Payload for POST /api/majors. Requires the CREATE_MAJOR permission.</summary>
public class CreateMajorRequest
{
    [Required(ErrorMessage = "MajorCode is required.")]
    public string MajorCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    public string Department { get; set; } = string.Empty;

    [Range(1, 300, ErrorMessage = "TotalCredits must be between 1 and 300.")]
    public int TotalCredits { get; set; }
}

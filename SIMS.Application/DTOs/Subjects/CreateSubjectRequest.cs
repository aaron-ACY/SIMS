using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Subjects;

/// <summary>
/// Payload for POST /api/subjects. Requires the CREATE_SUB permission.
/// </summary>
public class CreateSubjectRequest
{
    [Required(ErrorMessage = "SubjectCode is required.")]
    public string SubjectCode  { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    public string Name         { get; set; } = string.Empty;

    public string Description  { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10.")]
    public int Credits         { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    public string Department   { get; set; } = string.Empty;

    [Required(ErrorMessage = "Major is required.")]
    public string Major        { get; set; } = string.Empty;

    [Required(ErrorMessage = "AcademicYear is required.")]
    public string AcademicYear { get; set; } = string.Empty;

    [Range(1, 3, ErrorMessage = "Semester must be 1, 2 or 3.")]
    public int Semester        { get; set; }

    public bool IsRequired     { get; set; }
}

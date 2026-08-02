using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Classes;

/// <summary>
/// Payload for POST /api/classes. Requires the CREATE_CLASS permission.
/// </summary>
public class CreateClassRequest
{
    [Required(ErrorMessage = "ClassCode is required.")]
    public string ClassCode    { get; set; } = string.Empty;

    /// <summary>Subject.Id from subjects.csv.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "SubjectId is required.")]
    public int SubjectId       { get; set; }

    /// <summary>Instructor.Id from instructors.csv — not the underlying User.Id.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "InstructorId is required.")]
    public int InstructorId    { get; set; }

    [Range(1, 3, ErrorMessage = "Semester must be 1, 2 or 3.")]
    public int Semester        { get; set; }

    [Required(ErrorMessage = "AcademicYear is required.")]
    public string AcademicYear { get; set; } = string.Empty;

    public string Room         { get; set; } = string.Empty;
    public string Schedule     { get; set; } = string.Empty;

    [Range(1, 500, ErrorMessage = "MaxEnrollment must be between 1 and 500.")]
    public int MaxEnrollment   { get; set; }
}

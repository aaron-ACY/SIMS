using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Courses;

/// <summary>
/// Payload for POST /api/courses. Requires the CREATE_COURSE permission.
/// </summary>
public class CreateCourseRequest
{
    [Required(ErrorMessage = "CourseCode is required.")]
    public string CourseCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10.")]
    public int Credits { get; set; }

    /// <summary>Instructor.Id from instructors.csv — not the underlying User.Id.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "InstructorId is required.")]
    public int InstructorId { get; set; }

    [Range(1, 3, ErrorMessage = "Semester must be 1, 2 or 3.")]
    public int Semester { get; set; }

    [Required(ErrorMessage = "AcademicYear is required.")]
    public string AcademicYear { get; set; } = string.Empty;

    [Range(1, 500, ErrorMessage = "MaxEnrollment must be between 1 and 500.")]
    public int MaxEnrollment { get; set; }
}

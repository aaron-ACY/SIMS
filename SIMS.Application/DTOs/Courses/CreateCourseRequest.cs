using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Courses;

/// <summary>Payload for POST /api/courses. Requires the CREATE_COURSE permission.</summary>
public class CreateCourseRequest
{
    [Required(ErrorMessage = "CourseCode is required.")]
    public string CourseCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10.")]
    public int Credits { get; set; }

    public bool IsRequired { get; set; }
}

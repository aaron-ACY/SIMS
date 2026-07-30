namespace SIMS.Domain.Entities;

/// <summary>A course offered in a given semester, taught by one instructor.</summary>
public class Course
{
    public int Id { get; set; }

    /// <summary>Human-facing unique code, e.g. IT101.</summary>
    public string CourseCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }

    /// <summary>FK to instructors.csv (Instructor.Id, not User.Id).</summary>
    public int InstructorId { get; set; }

    public int Semester { get; set; }

    /// <summary>Academic year label, e.g. "2026-2027".</summary>
    public string AcademicYear { get; set; } = string.Empty;

    public int MaxEnrollment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

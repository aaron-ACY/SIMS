namespace SIMS.Application.DTOs.Grades;

/// <summary>Grade record returned by the Grades endpoints.</summary>
public class GradeResponse
{
    public int    Id             { get; set; }
    public int    EnrollmentId   { get; set; }
    public int    StudentId      { get; set; }

    /// <summary>Empty when the referenced student no longer exists.</summary>
    public string StudentName    { get; set; } = string.Empty;

    public int    ClassId        { get; set; }

    /// <summary>Empty when the referenced class no longer exists.</summary>
    public string ClassCode      { get; set; } = string.Empty;

    public double Score          { get; set; }
    public string Classification { get; set; } = string.Empty;

    /// <summary>
    /// Relative path to the submitted file inside Data/files/.
    /// Null until the student submits.
    /// </summary>
    public string? SubmissionPath { get; set; }

    /// <summary>Null until an instructor formally enters a score.</summary>
    public DateTime? GradedAt    { get; set; }
    public DateTime  UpdatedAt   { get; set; }
}

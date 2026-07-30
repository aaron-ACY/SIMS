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
    public DateTime GradedAt     { get; set; }
    public DateTime UpdatedAt    { get; set; }
}

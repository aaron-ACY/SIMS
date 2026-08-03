namespace SIMS.Application.DTOs.Classes;

/// <summary>
/// Simplified class information for GET /api/classes list view.
/// </summary>
public class ClassListItemResponse
{
    public int    Id                { get; set; }
    public string ClassCode         { get; set; } = string.Empty;
    public string SubjectName       { get; set; } = string.Empty;

    /// <summary>FK to instructors.csv (Instructor.Id). Use this for reliable filtering instead of name comparison.</summary>
    public int    InstructorId      { get; set; }

    public string InstructorCode    { get; set; } = string.Empty;
    public string InstructorName    { get; set; } = string.Empty;
    public int    MaxEnrollment     { get; set; }
    public int    CurrentEnrollment { get; set; }
    public bool   IsActive          { get; set; }
}

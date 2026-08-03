namespace SIMS.Application.DTOs.Classes;

/// <summary>
/// A class the current student is enrolled in.
/// Returned by GET /api/students/me/classes.
/// </summary>
public class StudentClassResponse
{
    /// <summary>
    /// The enrollment record ID.
    /// Pass this to POST /api/grades/{EnrollmentId}/submit to submit an assignment.
    /// </summary>
    public int    EnrollmentId   { get; set; }
    public int    ClassId        { get; set; }
    public string ClassCode      { get; set; } = string.Empty;
    public string SubjectName    { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public int    Semester       { get; set; }
    public string AcademicYear   { get; set; } = string.Empty;
    public string Room           { get; set; } = string.Empty;
    public string Schedule       { get; set; } = string.Empty;
    public bool   IsActive       { get; set; }
}

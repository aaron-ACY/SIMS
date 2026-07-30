namespace SIMS.Application.DTOs.Classes;

/// <summary>A class (lớp học), with the subject name and instructor name resolved for display.</summary>
public class ClassResponse
{
    public int    Id                { get; set; }
    public string ClassCode         { get; set; } = string.Empty;
    public int    SubjectId         { get; set; }

    /// <summary>Empty when the referenced subject no longer exists.</summary>
    public string SubjectName       { get; set; } = string.Empty;

    public int    InstructorId      { get; set; }

    /// <summary>Empty when the referenced instructor no longer exists.</summary>
    public string InstructorName    { get; set; } = string.Empty;

    public int    Semester          { get; set; }
    public string AcademicYear      { get; set; } = string.Empty;
    public string Room              { get; set; } = string.Empty;
    public string Schedule          { get; set; } = string.Empty;
    public int    MaxEnrollment     { get; set; }
    public int    CurrentEnrollment { get; set; }
    public bool   IsActive          { get; set; }
}

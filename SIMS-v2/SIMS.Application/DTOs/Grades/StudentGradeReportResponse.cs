namespace SIMS.Application.DTOs.Grades;

/// <summary>
/// Aggregated grade report for a student, returned by
/// GET /api/grades/student/{studentCode}.
/// Grades are grouped by the class they belong to so the frontend
/// can render a per-class breakdown.
/// </summary>
public class StudentGradeReportResponse
{
    public string StudentCode { get; set; } = string.Empty;
    public string FirstName   { get; set; } = string.Empty;
    public string LastName    { get; set; } = string.Empty;

    /// <summary>One entry per class the student has received grades in.</summary>
    public List<ClassGradeGroup> Classes { get; set; } = [];
}

/// <summary>All grades a student received within one class.</summary>
public class ClassGradeGroup
{
    public string ClassCode { get; set; } = string.Empty;
    public int    Semester  { get; set; }
    public List<GradeItemResponse> Grades { get; set; } = [];
}

/// <summary>One line in the grade report — one subject's result.</summary>
public class GradeItemResponse
{
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    /// <summary>Null when the grade has not yet been entered by an instructor.</summary>
    public double? Scores      { get; set; }

    /// <summary>Null when the grade has not yet been entered by an instructor.</summary>
    public string? Rating      { get; set; }
}

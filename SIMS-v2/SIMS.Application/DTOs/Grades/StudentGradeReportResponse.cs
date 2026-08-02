namespace SIMS.Application.DTOs.Grades;

/// <summary>
/// Aggregated grade report for a student, returned by
/// GET /api/grades/student/{studentCode}.
/// </summary>
public class StudentGradeReportResponse
{
    public string StudentCode { get; set; } = string.Empty;
    public string FirstName   { get; set; } = string.Empty;
    public string LastName    { get; set; } = string.Empty;

    /// <summary>ClassCode of the most-recent graded class.</summary>
    public string Class       { get; set; } = string.Empty;

    /// <summary>Semester of the most-recent graded class.</summary>
    public int    Semester    { get; set; }

    public List<GradeItemResponse> Grades { get; set; } = [];
}

/// <summary>One line in the grade report — one subject's result.</summary>
public class GradeItemResponse
{
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public double Scores      { get; set; }
    public string Rating      { get; set; } = string.Empty;
}

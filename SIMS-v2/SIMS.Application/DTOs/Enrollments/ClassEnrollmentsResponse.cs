namespace SIMS.Application.DTOs.Enrollments;

/// <summary>
/// Response for GET /api/classes/{classId}/enrollments — class info with enrolled students.
/// </summary>
public class ClassEnrollmentsResponse
{
    public string ClassCode     { get; set; } = string.Empty;
    public string SchoolYear    { get; set; } = string.Empty;
    public int    TotalStudents { get; set; }
    public List<EnrollmentItemResponse> Enrollments { get; set; } = [];
}

/// <summary>
/// One enrollment record with student details.
/// </summary>
public class EnrollmentItemResponse
{
    public int                    EnrollmentId { get; set; }
    public EnrollmentStudentInfo  Student      { get; set; } = new();
    public string                 Status       { get; set; } = string.Empty;
    public DateTime               EnrolledAt   { get; set; }
}

/// <summary>
/// Minimal student info for enrollment list.
/// </summary>
public class EnrollmentStudentInfo
{
    public string   StudentCode { get; set; } = string.Empty;
    public string   FullName    { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string   Gender      { get; set; } = string.Empty;
}

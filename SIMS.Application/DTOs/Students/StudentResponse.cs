namespace SIMS.Application.DTOs.Students;

/// <summary>
/// A student's academic record, with name and email resolved from the linked user
/// account so callers do not have to join against /api/users themselves.
/// </summary>
public class StudentResponse
{
    public int      Id             { get; set; }
    public int      UserId         { get; set; }
    public string   StudentCode    { get; set; } = string.Empty;
    public string   FullName       { get; set; } = string.Empty;
    public string   Email          { get; set; } = string.Empty;
    public DateTime DateOfBirth    { get; set; }
    public string   Gender         { get; set; } = string.Empty;
    public string   Phone          { get; set; } = string.Empty;
    public string   Address        { get; set; } = string.Empty;
    public string   Major          { get; set; } = string.Empty;
    public int      EnrollmentYear { get; set; }
    public string   Status         { get; set; } = string.Empty;
    public bool     IsActive       { get; set; }
}

namespace SIMS.Application.DTOs.Instructors;

/// <summary>
/// An instructor's employment record, with name and email resolved from the linked
/// user account so callers do not have to join against /api/users themselves.
/// </summary>
public class InstructorResponse
{
    public int      Id             { get; set; }
    public int      UserId         { get; set; }
    public string   InstructorCode { get; set; } = string.Empty;
    public string   FullName       { get; set; } = string.Empty;
    public string   Email          { get; set; } = string.Empty;
    public string   Department     { get; set; } = string.Empty;
    public string   Degree         { get; set; } = string.Empty;
    public string   Specialization { get; set; } = string.Empty;
    public DateTime HireDate       { get; set; }
    public string   Phone          { get; set; } = string.Empty;
    public bool     IsActive       { get; set; }
}

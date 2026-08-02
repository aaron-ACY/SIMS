namespace SIMS.Application.DTOs.Students;

/// <summary>
/// A student's academic record. Name and email are resolved from the linked user
/// account when the student has registered; otherwise they come from the imported profile.
/// </summary>
public class StudentResponse
{
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

    /// <summary>
    /// True when the student has created a login account (UserId is set).
    /// False for profiles that were imported but have not yet registered.
    /// </summary>
    public bool IsRegistered { get; set; }
}

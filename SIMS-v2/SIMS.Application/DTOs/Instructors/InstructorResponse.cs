namespace SIMS.Application.DTOs.Instructors;

/// <summary>
/// An instructor's employment record. Name and email are resolved from the linked
/// user account when the instructor has registered; otherwise they come from the imported profile.
/// </summary>
public class InstructorResponse
{
    public int      Id             { get; set; } 
    public string   InstructorCode { get; set; } = string.Empty;
    public string   FullName       { get; set; } = string.Empty;
    public string   Email          { get; set; } = string.Empty;
    public string   Department     { get; set; } = string.Empty;
    public string   Degree         { get; set; } = string.Empty;
    public string   Phone          { get; set; } = string.Empty;
    public bool     IsActive       { get; set; }

    /// <summary>
    /// True when the instructor has created a login account (UserId is set).
    /// False for profiles that were imported but have not yet registered.
    /// </summary>
    public bool IsRegistered { get; set; }
}

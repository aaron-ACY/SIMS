namespace SIMS.Domain.Constants;

/// <summary>
/// Canonical role name strings — use these everywhere instead of magic literals
/// so a rename is a single-file change.
/// </summary>
public static class Roles
{
    public const string Admin      = "Admin";
    public const string Instructor = "Instructor";
    public const string Student    = "Student";
}

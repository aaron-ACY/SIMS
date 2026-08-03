using System.Text.Json.Serialization;

namespace SIMS.Application.DTOs.Users;

/// <summary>
/// Profile of a user. Permissions are omitted — they are already embedded in
/// the JWT token and do not need to be repeated in every response.
/// Role-specific fields (Student / Instructor) are null-suppressed in JSON
/// so they only appear for the matching role.
/// </summary>
public class UserProfileResponse
{
    // ── Shared ──────────────────────────────────────────────────────────── //
    public string Username  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public string Role      { get; set; } = string.Empty;

    // ── Student-specific ─────────────────────────────────────────────────── //
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?    StudentCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime?  DateOfBirth { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?    Gender      { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?    Major       { get; set; }

    // ── Instructor-specific ──────────────────────────────────────────────── //
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?    InstructorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?    Department     { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?    Degree         { get; set; }

    // ── Shared optional (Student + Instructor) ──────────────────────────── //
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?    Phone { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string?    Address { get; set; }
}

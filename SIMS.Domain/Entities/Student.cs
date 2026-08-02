namespace SIMS.Domain.Entities;

/// <summary>
/// Academic record for a student.
///
/// A student profile can exist in two states:
///   1. Pre-registered (UserId is null): the profile was imported from a CSV by an admin.
///      Name and email are stored directly on the profile until the student registers.
///   2. Activated (UserId is set): the student has registered an account. Identity
///      (username, login credentials) lives on the linked <see cref="User"/>; FirstName,
///      LastName and Email on this entity are the authoritative copy used for display.
/// </summary>
public class Student
{
    public int Id { get; set; }

    /// <summary>
    /// FK to users.csv. Null when the profile was imported but the student has not yet
    /// registered an account.
    /// </summary>
    public int? UserId { get; set; }

    public string StudentCode { get; set; } = string.Empty;

    /// <summary>Set from the import CSV; copied to the User record on registration.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Set from the import CSV; copied to the User record on registration.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gmail used for registration lookup and as the User's email.</summary>
    public string Email { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Major { get; set; } = string.Empty;
    public int EnrollmentYear { get; set; }

    /// <summary>Enrollment state, e.g. Active, Suspended, Graduated.</summary>
    public string Status { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

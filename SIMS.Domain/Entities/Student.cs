namespace SIMS.Domain.Entities;

/// <summary>
/// Academic record for a student. Identity (username, email, name) lives on the
/// linked <see cref="User"/> via <see cref="UserId"/> and is not duplicated here.
/// </summary>
public class Student
{
    public int Id { get; set; }

    /// <summary>FK to users.csv — carries the name, email and login credentials.</summary>
    public int UserId { get; set; }

    public string StudentCode { get; set; } = string.Empty;
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

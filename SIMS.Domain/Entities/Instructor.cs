namespace SIMS.Domain.Entities;

/// <summary>
/// Employment record for an instructor. Identity (username, email, name) lives on
/// the linked <see cref="User"/> via <see cref="UserId"/> and is not duplicated here.
/// </summary>
public class Instructor
{
    public int Id { get; set; }

    /// <summary>FK to users.csv — carries the name, email and login credentials.</summary>
    public int UserId { get; set; }

    public string InstructorCode { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

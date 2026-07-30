namespace SIMS.Domain.Entities;

/// <summary>
/// A class (lớp học) — a scheduled instance of a subject, taught by one instructor.
/// </summary>
public class Class
{
    public int    Id                 { get; set; }

    /// <summary>Human-facing unique code, e.g. IT101-01.</summary>
    public string ClassCode          { get; set; } = string.Empty;

    /// <summary>FK to subjects.csv (Subject.Id).</summary>
    public int    SubjectId          { get; set; }

    /// <summary>FK to instructors.csv (Instructor.Id, not User.Id).</summary>
    public int    InstructorId       { get; set; }

    public int    Semester           { get; set; }

    /// <summary>Academic year label, e.g. "2026-2027".</summary>
    public string AcademicYear       { get; set; } = string.Empty;

    /// <summary>Room where the class is held, e.g. "A101".</summary>
    public string Room               { get; set; } = string.Empty;

    /// <summary>Human-readable schedule string, e.g. "Mon/Wed 08:00-09:30".</summary>
    public string Schedule           { get; set; } = string.Empty;

    public int    MaxEnrollment      { get; set; }
    public int    CurrentEnrollment  { get; set; }

    public bool     IsActive         { get; set; }
    public DateTime CreatedAt        { get; set; }
    public DateTime UpdatedAt        { get; set; }
}

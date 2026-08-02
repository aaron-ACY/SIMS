namespace SIMS.Domain.Entities;

/// <summary>A course (môn học) offered by the institution.</summary>
public class Course
{
    public int    Id          { get; set; }

    /// <summary>Human-facing unique code, e.g. CS101.</summary>
    public string CourseCode  { get; set; } = string.Empty;

    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int    Credits     { get; set; }

    /// <summary>True when this is a compulsory course.</summary>
    public bool   IsRequired  { get; set; }

    public bool     IsActive   { get; set; }
    public DateTime CreatedAt  { get; set; }
    public DateTime UpdatedAt  { get; set; }
}

namespace SIMS.Domain.Entities;

/// <summary>A subject (môn học) offered within a specific major and academic year.</summary>
public class Subject
{
    public int    Id           { get; set; }

    /// <summary>Human-facing unique code, e.g. IT101.</summary>
    public string SubjectCode  { get; set; } = string.Empty;

    public string Name         { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public int    Credits      { get; set; }
    public string Department   { get; set; } = string.Empty;
    public string Major        { get; set; } = string.Empty;

    /// <summary>Academic year label, e.g. "2026-2027".</summary>
    public string AcademicYear { get; set; } = string.Empty;

    public int    Semester     { get; set; }

    /// <summary>True when this is a compulsory subject.</summary>
    public bool   IsRequired   { get; set; }

    public bool     IsActive   { get; set; }
    public DateTime CreatedAt  { get; set; }
    public DateTime UpdatedAt  { get; set; }
}

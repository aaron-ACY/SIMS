namespace SIMS.Domain.Entities;

public class Subject
{
    public int    Id           { get; set; }

    public string SubjectCode  { get; set; } = string.Empty;

    public string Name         { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public int    Credits      { get; set; }
    public string Department   { get; set; } = string.Empty;
    public string Major        { get; set; } = string.Empty;

    public string AcademicYear { get; set; } = string.Empty;

    public int    Semester     { get; set; }

    public bool   IsRequired   { get; set; }

    public bool     IsActive   { get; set; }
    public DateTime CreatedAt  { get; set; }
    public DateTime UpdatedAt  { get; set; }
}

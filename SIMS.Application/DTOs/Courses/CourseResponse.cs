namespace SIMS.Application.DTOs.Courses;

/// <summary>A course, with the teaching instructor's name resolved for display.</summary>
public class CourseResponse
{
    public int    Id             { get; set; }
    public string CourseCode     { get; set; } = string.Empty;
    public string Name           { get; set; } = string.Empty;
    public string Description    { get; set; } = string.Empty;
    public int    Credits        { get; set; }
    public int    InstructorId   { get; set; }

    /// <summary>Empty when the course points at an instructor that no longer exists.</summary>
    public string InstructorName { get; set; } = string.Empty;

    public int    Semester       { get; set; }
    public string AcademicYear   { get; set; } = string.Empty;
    public int    MaxEnrollment  { get; set; }
    public bool   IsActive       { get; set; }
}

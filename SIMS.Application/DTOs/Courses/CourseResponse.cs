namespace SIMS.Application.DTOs.Courses;

/// <summary>A course (môn học), returned by the API.</summary>
public class CourseResponse
{
    public int    Id          { get; set; }
    public string CourseCode  { get; set; } = string.Empty;
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int    Credits     { get; set; }
    public bool   IsRequired  { get; set; }
    public bool   IsActive    { get; set; }
}

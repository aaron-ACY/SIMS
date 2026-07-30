namespace SIMS.Domain.Entities;

/// <summary>Represents a system role (Admin, Instructor, Student).</summary>
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

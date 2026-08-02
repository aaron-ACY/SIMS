namespace SIMS.Domain.Entities;

/// <summary>Represents a fine-grained permission (e.g., students:read).</summary>
public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

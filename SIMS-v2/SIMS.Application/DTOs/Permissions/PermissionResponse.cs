namespace SIMS.Application.DTOs.Permissions;

/// <summary>A permission as stored in permissions.csv.</summary>
public class PermissionResponse
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

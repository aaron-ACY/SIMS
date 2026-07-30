namespace SIMS.Application.DTOs.Permissions;

/// <summary>The full permission set a role holds after an assignment.</summary>
public class RolePermissionsResponse
{
    public int    RoleId   { get; set; }
    public string RoleName { get; set; } = string.Empty;

    public IReadOnlyList<PermissionResponse> Permissions { get; set; } = [];
}

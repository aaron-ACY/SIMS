using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Permissions;

/// <summary>
/// Payload for POST /api/roles/{roleId}/permissions.
/// Requires the GET_PERMISSION permission.
/// </summary>
public class AssignPermissionRequest
{
    /// <summary>Permission.Id from permissions.csv.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "PermissionId is required.")]
    public int PermissionId { get; set; }
}

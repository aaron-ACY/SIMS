using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Permissions;

/// <summary>
/// Payload for PUT /api/permissions/{id}. Requires the EDIT_PERMISSION permission.
/// Both fields are optional — omit one to leave it unchanged.
/// </summary>
public class UpdatePermissionRequest
{
    /// <summary>New name, or null to keep the current one. Must stay unique.</summary>
    [MaxLength(64, ErrorMessage = "Name must be at most 64 characters long.")]
    public string? Name { get; set; }

    /// <summary>New description, or null to keep the current one.</summary>
    [MaxLength(256, ErrorMessage = "Description must be at most 256 characters long.")]
    public string? Description { get; set; }
}

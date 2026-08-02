using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Permissions;

/// <summary>
/// Payload for POST /api/permissions. Requires the CREATE_PERMISSION permission.
/// </summary>
public class CreatePermissionRequest
{
    /// <summary>
    /// Permission name. Stored upper-cased and must be unique. Note that creating a
    /// row here does not register an authorization policy — see PermissionService.
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(64, ErrorMessage = "Name must be at most 64 characters long.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(256, ErrorMessage = "Description must be at most 256 characters long.")]
    public string Description { get; set; } = string.Empty;
}

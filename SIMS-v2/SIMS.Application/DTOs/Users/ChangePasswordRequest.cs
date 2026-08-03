using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Users;

/// <summary>
/// Payload for PUT /api/users/change-password.
/// Requires the CHANGE_PASSWORD permission (Instructor and Student roles).
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>The caller's current password — used to verify identity before changing.</summary>
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>The new password. Must be at least 8 characters long.</summary>
    [Required]
    [MinLength(8, ErrorMessage = "New password must be at least 8 characters long.")]
    public string NewPassword { get; set; } = string.Empty;
}

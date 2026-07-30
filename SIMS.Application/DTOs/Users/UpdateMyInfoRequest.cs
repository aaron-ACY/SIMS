using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Users;

/// <summary>
/// Payload for PUT /api/users/me. Requires the EDIT_INFO permission.
/// Only fields the caller owns are editable here — role and active state are not.
/// </summary>
public class UpdateMyInfoRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "FirstName is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName is required.")]
    public string LastName { get; set; } = string.Empty;
}

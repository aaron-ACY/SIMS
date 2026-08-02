using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Auth;

/// <summary>
/// Payload for self-service registration.
/// The caller must supply a Gmail that was imported by an admin;
/// the backend uses it to locate the student or instructor profile and
/// determine which role to assign.
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required.")]
    [MinLength(6, ErrorMessage = "Username must be at least 6 characters long.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;
}

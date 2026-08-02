using System.ComponentModel.DataAnnotations;
using SIMS.Application.Validation;

namespace SIMS.Application.DTOs.Users;

/// <summary>
/// Payload for POST /api/users/instructor.
/// Creates a user account (role = Instructor) and the linked instructor profile atomically.
/// Requires the CREATE_USER permission.
/// </summary>
public class CreateInstructorUserRequest
{
    // ── Account credentials ───────────────────────────────────────────── //

    [Required(ErrorMessage = "Username is required.")]
    [MinLength(6, ErrorMessage = "Username must be at least 6 characters long.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "FirstName is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName is required.")]
    public string LastName { get; set; } = string.Empty;

    // ── Instructor profile ────────────────────────────────────────────── //

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(100, ErrorMessage = "Department must not exceed 100 characters.")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Degree is required.")]
    [StringLength(50, ErrorMessage = "Degree must not exceed 50 characters.")]
    public string Degree { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string Phone { get; set; } = string.Empty;
}

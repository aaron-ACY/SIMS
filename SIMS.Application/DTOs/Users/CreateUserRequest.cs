using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Users;

/// <summary>
/// Payload for POST /api/users. Requires the CREATE_USER permission.
/// </summary>
public class CreateUserRequest
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "FirstName is required.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName is required.")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Role to assign: 1 = Admin, 2 = Instructor, 3 = Student.</summary>
    [Required(ErrorMessage = "RoleId is required.")]
    public int RoleId { get; set; }
}

using System.ComponentModel.DataAnnotations;
using SIMS.Application.Validation;

namespace SIMS.Application.DTOs.Users;

/// <summary>
/// Payload for POST /api/users/student.
/// Creates a user account (role = Student) and the linked student profile atomically.
/// Requires the CREATE_USER permission.
/// </summary>
public class CreateStudentUserRequest
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

    // ── Student profile ───────────────────────────────────────────────── //

    [Required(ErrorMessage = "Date of birth is required.")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [StringLength(10, ErrorMessage = "Gender must not exceed 10 characters.")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(200, ErrorMessage = "Address must not exceed 200 characters.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Major is required.")]
    [StringLength(100, ErrorMessage = "Major must not exceed 100 characters.")]
    public string Major { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enrollment year is required.")]
    [Range(1900, 2100, ErrorMessage = "Enrollment year must be between 1900 and 2100.")]
    public int EnrollmentYear { get; set; }

    [StringLength(50, ErrorMessage = "Status must not exceed 50 characters.")]
    public string Status { get; set; } = "Active";
}

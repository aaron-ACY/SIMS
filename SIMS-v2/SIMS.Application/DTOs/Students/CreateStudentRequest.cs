using System.ComponentModel.DataAnnotations;
using SIMS.Application.Validation;

namespace SIMS.Application.DTOs.Students;

public class CreateStudentRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Student code is required")]
    [StudentCode]
    public string StudentCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    [StringLength(10, ErrorMessage = "Gender must not exceed 10 characters")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200, ErrorMessage = "Address must not exceed 200 characters")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Major is required")]
    [StringLength(100, ErrorMessage = "Major must not exceed 100 characters")]
    public string Major { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enrollment year is required")]
    [Range(1900, 2100, ErrorMessage = "Enrollment year must be between 1900 and 2100")]
    public int EnrollmentYear { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [StringLength(50, ErrorMessage = "Status must not exceed 50 characters")]
    public string Status { get; set; } = "Active";
}

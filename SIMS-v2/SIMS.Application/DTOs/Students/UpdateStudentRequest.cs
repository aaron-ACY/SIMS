using System.ComponentModel.DataAnnotations;
using SIMS.Application.Validation;

namespace SIMS.Application.DTOs.Students;

public class UpdateStudentRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Student code is required")]
    [StudentCode]
    public string? StudentCode { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(10, ErrorMessage = "Gender must not exceed 10 characters")]
    public string? Gender { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }

    [StringLength(200, ErrorMessage = "Address must not exceed 200 characters")]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = "Major must not exceed 100 characters")]
    public string? Major { get; set; }

    [Range(1900, 2100, ErrorMessage = "Enrollment year must be between 1900 and 2100")]
    public int? EnrollmentYear { get; set; }

    [StringLength(50, ErrorMessage = "Status must not exceed 50 characters")]
    public string? Status { get; set; }
}

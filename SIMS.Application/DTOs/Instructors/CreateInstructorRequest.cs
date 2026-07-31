using System.ComponentModel.DataAnnotations;
using SIMS.Application.Validation;

namespace SIMS.Application.DTOs.Instructors;

public class CreateInstructorRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Instructor code is required")]
    [InstructorCode]
    public string InstructorCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required")]
    [StringLength(100, ErrorMessage = "Department must not exceed 100 characters")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Degree is required")]
    [StringLength(50, ErrorMessage = "Degree must not exceed 50 characters")]
    public string Degree { get; set; } = string.Empty;

    [Required(ErrorMessage = "Specialization is required")]
    [StringLength(100, ErrorMessage = "Specialization must not exceed 100 characters")]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hire date is required")]
    public DateTime HireDate { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string Phone { get; set; } = string.Empty;
}

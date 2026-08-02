using System.ComponentModel.DataAnnotations;
using SIMS.Application.Validation;

namespace SIMS.Application.DTOs.Instructors;

public class UpdateInstructorRequest
{
    [InstructorCode]
    public string? InstructorCode { get; set; }

    [StringLength(100, ErrorMessage = "Department must not exceed 100 characters")]
    public string? Department { get; set; }

    [StringLength(50, ErrorMessage = "Degree must not exceed 50 characters")]
    public string? Degree { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? Phone { get; set; }
}

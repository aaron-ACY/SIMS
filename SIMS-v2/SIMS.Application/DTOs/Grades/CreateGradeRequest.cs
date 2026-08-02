using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Grades;

/// <summary>
/// Payload for POST /api/grades. Requires the ENTER_GRADE permission.
/// </summary>
public class CreateGradeRequest
{
    /// <summary>Enrollment.Id from enrollments.csv.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "EnrollmentId is required.")]
    public int EnrollmentId { get; set; }

    [Range(0.0, 10.0, ErrorMessage = "Score must be between 0 and 10.")]
    public double Score { get; set; }
}

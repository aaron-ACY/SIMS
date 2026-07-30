using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Grades;

/// <summary>
/// Payload for PUT /api/grades/{id}. Requires the EDIT_GRADE permission.
/// </summary>
public class UpdateGradeRequest
{
    [Range(0.0, 10.0, ErrorMessage = "Score must be between 0 and 10.")]
    public double Score { get; set; }
}

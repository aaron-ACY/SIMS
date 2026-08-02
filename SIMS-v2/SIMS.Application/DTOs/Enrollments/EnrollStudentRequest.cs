using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Enrollments;

/// <summary>
/// Payload for POST /api/classes/{classId}/enrollments. Requires the ENROLLMENTS permission.
/// </summary>
public class EnrollStudentRequest
{
    /// <summary>Student.Id from students.csv.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "StudentId is required.")]
    public int StudentId { get; set; }
}

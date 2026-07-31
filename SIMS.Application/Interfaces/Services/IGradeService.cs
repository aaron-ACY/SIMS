using SIMS.Application.DTOs.Grades;

namespace SIMS.Application.Interfaces.Services;

public interface IGradeService
{
    /// <summary>Enters a grade for a student's enrollment. Requires CREATE_GRADE.</summary>
    Task<GradeResponse> CreateAsync(CreateGradeRequest request);

    /// <summary>Edits an existing grade. Requires EDIT_GRADE.</summary>
    Task<GradeResponse> UpdateAsync(int gradeId, UpdateGradeRequest request);

    /// <summary>
    /// Returns the aggregated grade report for the student identified by
    /// <paramref name="studentCode"/>. Throws when the student does not exist.
    /// Requires VIEW_SCORE.
    /// </summary>
    Task<StudentGradeReportResponse> GetScoresByStudentCodeAsync(string studentCode);
}

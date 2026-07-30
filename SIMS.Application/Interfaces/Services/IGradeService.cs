using SIMS.Application.DTOs.Grades;

namespace SIMS.Application.Interfaces.Services;

public interface IGradeService
{
    /// <summary>Enters a grade for a student's enrollment. Requires CREATE_GRADE.</summary>
    Task<GradeResponse> CreateAsync(CreateGradeRequest request);

    /// <summary>Edits an existing grade. Requires EDIT_GRADE.</summary>
    Task<GradeResponse> UpdateAsync(int gradeId, UpdateGradeRequest request);

    /// <summary>
    /// Returns all grades for the student linked to <paramref name="userId"/>.
    /// Throws when the user does not exist or the account is not a student. Requires VIEW_SCORE.
    /// </summary>
    Task<IEnumerable<GradeResponse>> GetScoresByUserIdAsync(int userId);
}

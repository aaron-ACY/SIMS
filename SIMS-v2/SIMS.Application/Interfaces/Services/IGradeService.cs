using SIMS.Application.DTOs.Grades;

namespace SIMS.Application.Interfaces.Services;

public interface IGradeService
{
    /// <summary>
    /// Records a student's assignment submission for an enrollment.
    /// Creates a grade record (without a score) if one does not exist yet,
    /// or updates the submission path on re-submission.
    /// Requires the SUBMITTED permission (Student role).
    /// </summary>
    Task<GradeResponse> SubmitAsync(int enrollmentId, string submissionPath, CancellationToken ct = default);

    /// <summary>
    /// Enters a score for an enrollment that already has a submission.
    /// Throws when no submission exists (SubmissionPath is null) or when
    /// a grade has already been entered for the enrollment.
    /// Requires ENTER_GRADE.
    /// </summary>
    Task<GradeResponse> CreateAsync(CreateGradeRequest request, CancellationToken ct = default);

    /// <summary>
    /// Edits the score of a grade that has already been formally entered.
    /// Throws when the grade has never been entered (GradedAt is null).
    /// Requires EDIT_GRADE.
    /// </summary>
    Task<GradeResponse> UpdateAsync(int gradeId, UpdateGradeRequest request, CancellationToken ct = default);

    /// <summary>
    /// Returns the aggregated grade report for the student identified by
    /// <paramref name="studentCode"/>. Throws when the student does not exist.
    /// Requires VIEW_SCORE.
    /// </summary>
    Task<StudentGradeReportResponse> GetScoresByStudentCodeAsync(string studentCode, CancellationToken ct = default);
    /// <summary>
    /// Returns all grade/submission records for every enrollment in the given class.
    /// Requires VIEW_CLASS_GRADES.
    /// </summary>
    Task<IEnumerable<GradeResponse>> GetGradesByClassIdAsync(int classId, CancellationToken ct = default);
}

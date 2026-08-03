namespace SIMS.Domain.Entities;

/// <summary>Grade record for a student's enrolment in a class.</summary>
public class Grade
{
    public int    Id           { get; set; }

    /// <summary>FK to enrollments.csv (Enrollment.Id).</summary>
    public int    EnrollmentId { get; set; }

    /// <summary>FK to students.csv — denormalised for quick lookup.</summary>
    public int    StudentId    { get; set; }

    /// <summary>FK to classes.csv — denormalised for quick lookup.</summary>
    public int    ClassId      { get; set; }

    /// <summary>
    /// Numeric score in the range 0–10.
    /// Null until an instructor formally enters a score.
    /// </summary>
    public double? Score        { get; set; }

    /// <summary>
    /// Classification derived from Score:
    /// Refer (&lt;6.5) | Pass (6.5–7.9) | Merit (8–8.9) | Distinction (9–10).
    /// Null until an instructor formally enters a score.
    /// </summary>
    public string? Classification { get; set; }

    /// <summary>
    /// Relative path to the submitted assignment file inside Data/files/.
    /// Null until the student submits via POST /api/grades/{enrollmentId}/submit.
    /// </summary>
    public string? SubmissionPath { get; set; }

    /// <summary>
    /// Set when an instructor formally enters a score. Null until graded,
    /// which means ENTER_GRADE has not yet been called for this enrollment.
    /// </summary>
    public DateTime? GradedAt  { get; set; }
    public DateTime  UpdatedAt { get; set; }
}

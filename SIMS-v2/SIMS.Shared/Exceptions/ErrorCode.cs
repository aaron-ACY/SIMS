using System.Net;

namespace SIMS.Shared.Exceptions;

/// <summary>
/// Catalogue of every business error the API can return.
/// Each entry carries a stable numeric code, a default message and the
/// HTTP status the response should use.
/// </summary>
public sealed class ErrorCode
{
    public int Code { get; }
    public string Message { get; }
    public HttpStatusCode StatusCode { get; }

    private ErrorCode(int code, string message, HttpStatusCode statusCode)
    {
        Code = code;
        Message = message;
        StatusCode = statusCode;
    }

    // ── Generic ───────────────────────────────────────────────────────── //

    public static readonly ErrorCode UNCATEGORIZED_EXCEPTION =
        new(9999, "Uncategorized error", HttpStatusCode.InternalServerError);

    public static readonly ErrorCode INVALID_KEY =
        new(1001, "Invalid message key", HttpStatusCode.BadRequest);

    // ── User / account ────────────────────────────────────────────────── //

    public static readonly ErrorCode USER_EXISTED =
        new(1002, "User existed", HttpStatusCode.BadRequest);

    public static readonly ErrorCode USERNAME_INVALID =
        new(1003, "Username must be at least 6 characters long", HttpStatusCode.BadRequest);

    public static readonly ErrorCode INVALID_PASSWORD =
        new(1004, "Password must be at least 8 characters long", HttpStatusCode.BadRequest);

    public static readonly ErrorCode USER_NOT_EXISTED =
        new(1005, "User not existed", HttpStatusCode.NotFound);

    public static readonly ErrorCode EMAIL_EXISTED =
        new(1012, "Email existed", HttpStatusCode.BadRequest);

    public static readonly ErrorCode ROLE_NOT_EXISTED =
        new(1013, "Role not existed", HttpStatusCode.BadRequest);

    public static readonly ErrorCode CANNOT_DELETE_SELF =
        new(1014, "You cannot delete your own account", HttpStatusCode.BadRequest);

    public static readonly ErrorCode WRONG_CURRENT_PASSWORD =
        new(1044, "Current password is incorrect", HttpStatusCode.BadRequest);

    public static readonly ErrorCode USER_HAS_ACTIVE_ENROLLMENTS =
        new(1033, "Cannot delete student — they have active class enrollments",
            HttpStatusCode.Conflict);

    public static readonly ErrorCode USER_HAS_ACTIVE_CLASSES =
        new(1034, "Cannot delete instructor — they are assigned to active classes",
            HttpStatusCode.Conflict);

    // ── Course ────────────────────────────────────────────────────────── //

    public static readonly ErrorCode COURSE_NOT_EXISTED =
        new(1015, "Course not existed", HttpStatusCode.NotFound);

    public static readonly ErrorCode COURSE_CODE_EXISTED =
        new(1016, "Course code existed", HttpStatusCode.BadRequest);

    // ── Major ─────────────────────────────────────────────────────────── //

    public static readonly ErrorCode MAJOR_NOT_EXISTED =
        new(1042, "Major not existed", HttpStatusCode.NotFound);

    public static readonly ErrorCode MAJOR_CODE_EXISTED =
        new(1043, "Major code existed", HttpStatusCode.BadRequest);

    public static readonly ErrorCode INSTRUCTOR_NOT_EXISTED =
        new(1017, "Instructor not existed", HttpStatusCode.NotFound);

    public static readonly ErrorCode INSTRUCTOR_CODE_EXISTED =
        new(1035, "Instructor code already exists", HttpStatusCode.BadRequest);

    // ── Subject ───────────────────────────────────────────────────────── //

    public static readonly ErrorCode SUBJECT_NOT_EXISTED =
        new(1022, "Subject not existed", HttpStatusCode.NotFound);

    public static readonly ErrorCode SUBJECT_CODE_EXISTED =
        new(1023, "Subject code existed", HttpStatusCode.BadRequest);

    // ── Class ─────────────────────────────────────────────────────────── //

    public static readonly ErrorCode CLASS_NOT_EXISTED =
        new(1024, "Class not existed", HttpStatusCode.NotFound);

    public static readonly ErrorCode CLASS_CODE_EXISTED =
        new(1025, "Class code existed", HttpStatusCode.BadRequest);

    public static readonly ErrorCode CLASS_FULL =
        new(1026, "Class has reached maximum enrollment", HttpStatusCode.BadRequest);

    // ── Enrollment ────────────────────────────────────────────────────── //

    public static readonly ErrorCode ALREADY_ENROLLED =
        new(1027, "Student is already enrolled in this class", HttpStatusCode.BadRequest);

    public static readonly ErrorCode ENROLLMENT_NOT_EXISTED =
        new(1028, "Enrollment not found", HttpStatusCode.NotFound);

    public static readonly ErrorCode STUDENT_NOT_EXISTED =
        new(1029, "Student not existed", HttpStatusCode.NotFound);

    public static readonly ErrorCode STUDENT_CODE_EXISTED =
        new(1032, "Student code already exists", HttpStatusCode.BadRequest);

    // ── Registration ──────────────────────────────────────────────────── //

    /// <summary>Email was not found in the students or instructors import list.</summary>
    public static readonly ErrorCode EMAIL_NOT_REGISTERED =
        new(1037, "This email is not registered in the system. Please contact an administrator.",
            HttpStatusCode.UnprocessableEntity);

    /// <summary>A user account is already linked to this profile's email.</summary>
    public static readonly ErrorCode ACCOUNT_ALREADY_LINKED =
        new(1038, "An account already exists for this email.",
            HttpStatusCode.BadRequest);

    // ── CSV Import ────────────────────────────────────────────────────── //

    /// <summary>The uploaded file could not be parsed as a valid CSV.</summary>
    public static readonly ErrorCode CSV_PARSE_ERROR =
        new(1039, "The uploaded file could not be parsed. Check that it is a valid CSV.",
            HttpStatusCode.UnprocessableEntity);

    // ── Grade ─────────────────────────────────────────────────────────── //

    public static readonly ErrorCode GRADE_NOT_EXISTED =
        new(1030, "Grade not found", HttpStatusCode.NotFound);

    public static readonly ErrorCode GRADE_ALREADY_EXISTS =
        new(1031, "A grade already exists for this student in this class",
            HttpStatusCode.BadRequest);

    /// <summary>
    /// Raised when an instructor tries to enter a grade but the student
    /// has not yet submitted an assignment (SubmissionPath is null).
    /// </summary>
    public static readonly ErrorCode SUBMISSION_NOT_FOUND =
        new(1040, "No submission found for this enrollment. The student must submit an assignment before a grade can be entered.",
            HttpStatusCode.UnprocessableEntity);

    /// <summary>
    /// Raised when an instructor tries to edit a grade that has never been
    /// formally entered (GradedAt is null).
    /// </summary>
    public static readonly ErrorCode GRADE_NOT_YET_ENTERED =
        new(1041, "This grade has not been entered yet and cannot be edited.",
            HttpStatusCode.UnprocessableEntity);

    // ── Permission ────────────────────────────────────────────────────── //

    public static readonly ErrorCode PERMISSION_NOT_EXISTED =
        new(1018, "Permission not existed", HttpStatusCode.NotFound);

    public static readonly ErrorCode PERMISSION_EXISTED =
        new(1019, "Permission name existed", HttpStatusCode.BadRequest);

    public static readonly ErrorCode PERMISSION_ALREADY_ASSIGNED =
        new(1020, "Role already has this permission", HttpStatusCode.BadRequest);

    // ── Authentication / authorization ────────────────────────────────── //

    public static readonly ErrorCode UNAUTHENTICATED =
        new(1006, "Unauthenticated", HttpStatusCode.Unauthorized);

    public static readonly ErrorCode UNAUTHORIZED =
        new(1007, "You do not have permission", HttpStatusCode.Forbidden);

    public static readonly ErrorCode INVALID_CREDENTIALS =
        new(1008, "Invalid credentials", HttpStatusCode.Unauthorized);

    public static readonly ErrorCode INVALID_TOKEN =
        new(1009, "Token is invalid or has been revoked", HttpStatusCode.Unauthorized);

    public static readonly ErrorCode REFRESH_WINDOW_EXPIRED =
        new(1021, "Token is too old to refresh — please log in again",
            HttpStatusCode.Unauthorized);

    // ── Rate limiting ─────────────────────────────────────────────────── //

    public static readonly ErrorCode TOO_MANY_REQUESTS =
        new(1036, "Too many requests — please try again later",
            HttpStatusCode.TooManyRequests);

    // ── Validation ────────────────────────────────────────────────────── //

    public static readonly ErrorCode VALIDATION_ERROR =
        new(1010, "One or more validation errors occurred", HttpStatusCode.UnprocessableEntity);

    public static readonly ErrorCode RESOURCE_NOT_FOUND =
        new(1011, "Requested resource was not found", HttpStatusCode.NotFound);
}

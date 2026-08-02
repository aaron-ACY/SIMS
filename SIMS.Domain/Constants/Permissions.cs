namespace SIMS.Domain.Constants;

/// <summary>
/// Canonical permission name strings — these must match the Name column in
/// Data/permissions.csv exactly, since that is what gets written into the
/// "permission" claim at login and compared by the authorization policies.
/// </summary>
public static class Permissions
{
    // ── Users ─────────────────────────────────────────────────────────── //
    public const string ViewUsers    = "VIEW_USERS";
    public const string EditProfile  = "EDIT_PROFILE";
    public const string CreateUser   = "CREATE_USER";
    public const string DeleteUser   = "DELETE_USER";

    // ── Students ──────────────────────────────────────────────────────── //
    public const string ViewStudents    = "VIEW_STUDENTS";
    public const string CreateStudent   = "CREATE_STUDENT";
    public const string EditStudent     = "EDIT_STUDENT";
    public const string DeleteStudent   = "DELETE_STUDENT";
    public const string ImportStudents  = "IMPORT_STUDENTS";

    // ── Instructors ───────────────────────────────────────────────────── //
    public const string ViewInstructors    = "VIEW_INSTRUCTORS";
    public const string CreateInstructor   = "CREATE_INSTRUCTOR";
    public const string EditInstructor     = "EDIT_INSTRUCTOR";
    public const string DeleteInstructor   = "DELETE_INSTRUCTOR";
    public const string ImportInstructors  = "IMPORT_INSTRUCTORS";

    // ── Majors (Chuyên ngành) ─────────────────────────────────────────── //
    public const string ViewMajors  = "VIEW_MAJOR";
    public const string CreateMajor = "CREATE_MAJOR";
    public const string DeleteMajor = "DELETE_MAJOR";

    // ── Courses (Môn học) ─────────────────────────────────────────────── //
    public const string ViewCourses  = "VIEW_COURSES";
    public const string CreateCourse = "CREATE_COURSE";
    public const string DeleteCourse = "DELETE_COURSE";

    // ── Subjects ──────────────────────────────────────────────────────── //
    public const string ViewSubject   = "VIEW_SUB";
    public const string CreateSubject = "CREATE_SUB";
    public const string DeleteSubject = "DELETE_SUB";

    // ── Classes ───────────────────────────────────────────────────────── //
    public const string ViewClasses           = "VIEW_CLA";
    public const string ListStudentsInClass   = "LIST_STU";
    public const string CreateClass           = "CREATE_CLASS";
    public const string Enrollments           = "ENROLLMENTS";
    public const string GetOut                = "GETOUT";

    // ── Grades ────────────────────────────────────────────────────────── //
    public const string EnterGrade  = "ENTER_GRADE";
    public const string EditGrade   = "EDIT_GRADE";
    public const string ViewScore   = "VIEW_SCORE";

    /// <summary>
    /// Allows a student to submit an assignment file for an enrollment.
    /// Must be held before an instructor can enter a grade.
    /// </summary>
    public const string Submitted   = "SUBMITTED";

    // ── Permissions management ────────────────────────────────────────── //
    public const string ViewPermissions  = "VIEW_PERMISSIONS";
    public const string CreatePermission = "CREATE_PERMISSION";
    public const string EditPermission   = "EDIT_PERMISSION";

    /// <summary>
    /// Guards assigning a permission to a role. Named GET_PERMISSION to match
    /// permissions.csv, though the operation it protects is a write.
    /// </summary>
    public const string GetPermission = "GET_PERMISSION";

    /// <summary>
    /// Every permission the API knows about. Program.cs registers one
    /// authorization policy per entry, so a new permission only needs adding here
    /// (plus permissions.csv) to become usable in [Authorize(Policy = ...)].
    /// </summary>
    public static readonly IReadOnlyList<string> All =
    [
        ViewUsers,
        CreateUser,
        DeleteUser,
        ViewStudents,
        CreateStudent,
        EditStudent,
        DeleteStudent,
        ImportStudents,
        ViewInstructors,
        CreateInstructor,
        EditInstructor,
        DeleteInstructor,
        ImportInstructors,
        ViewMajors,
        CreateMajor,
        DeleteMajor,
        ViewCourses,
        CreateCourse,
        DeleteCourse,
        ViewPermissions,
        CreatePermission,
        EditPermission,
        GetPermission,
        ViewSubject,
        CreateSubject,
        DeleteSubject,
        ViewClasses,
        ListStudentsInClass,
        CreateClass,
        Enrollments,
        GetOut,
        EnterGrade,
        EditGrade,
        ViewScore,
        Submitted,
        EditProfile
    ];
}

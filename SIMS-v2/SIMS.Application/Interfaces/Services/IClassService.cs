using SIMS.Application.DTOs.Classes;
using SIMS.Application.DTOs.Enrollments;

namespace SIMS.Application.Interfaces.Services;

public interface IClassService
{
    /// <summary>Returns all active and inactive classes. Requires the VIEW_CLA permission.</summary>
    Task<IEnumerable<ClassListItemResponse>> GetClassesAsync(CancellationToken ct = default);

    /// <summary>Returns class info with enrolled students. Requires the LIST_STU permission.</summary>
    Task<ClassEnrollmentsResponse> GetStudentsInClassAsync(int classId, CancellationToken ct = default);

    /// <summary>Creates a class. Requires the CREATE_CLASS permission.</summary>
    Task<ClassResponse> CreateAsync(CreateClassRequest request, CancellationToken ct = default);

    /// <summary>Enrolls a student into a class. Requires the ENROLLMENTS permission.</summary>
    Task EnrollStudentAsync(int classId, EnrollStudentRequest request, CancellationToken ct = default);

    /// <summary>Removes a student from a class. Requires the GETOUT permission.</summary>
    Task RemoveStudentAsync(int classId, int studentId, CancellationToken ct = default);
}

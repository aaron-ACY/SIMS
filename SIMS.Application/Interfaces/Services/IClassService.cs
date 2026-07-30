using SIMS.Application.DTOs.Classes;
using SIMS.Application.DTOs.Enrollments;

namespace SIMS.Application.Interfaces.Services;

public interface IClassService
{
    /// <summary>Creates a class. Requires the CREATE_CLASS permission.</summary>
    Task<ClassResponse> CreateAsync(CreateClassRequest request);

    /// <summary>Enrolls a student into a class. Requires the ENROLLMENTS permission.</summary>
    Task EnrollStudentAsync(int classId, EnrollStudentRequest request);

    /// <summary>Removes a student from a class. Requires the GETOUT permission.</summary>
    Task RemoveStudentAsync(int classId, int studentId);
}

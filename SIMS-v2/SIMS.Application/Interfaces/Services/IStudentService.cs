using SIMS.Application.DTOs.Students;

namespace SIMS.Application.Interfaces.Services;

public interface IStudentService
{
    Task<IEnumerable<StudentResponse>> GetAllAsync(CancellationToken ct = default);
    Task<StudentResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken ct = default);
    Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Bulk-imports student profiles from a CSV stream.
    /// Expected columns (no header required): StudentCode, FirstName, LastName,
    /// DateOfBirth, Gender, Phone, City, Country, Email, Major.
    /// </summary>
    Task<ImportStudentsResponse> ImportAsync(Stream csvStream, CancellationToken ct = default);
}

using SIMS.Application.DTOs.Instructors;

namespace SIMS.Application.Interfaces.Services;

public interface IInstructorService
{
    /// <summary>Returns every instructor with name and email resolved from the linked user.</summary>
    Task<IEnumerable<InstructorResponse>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Returns a single instructor by internal ID. Throws when not found.</summary>
    Task<InstructorResponse> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Creates a new instructor record linked to an existing user.</summary>
    Task<InstructorResponse> CreateAsync(CreateInstructorRequest request, CancellationToken ct = default);

    /// <summary>Applies a partial update to an existing instructor. Throws when not found.</summary>
    Task<InstructorResponse> UpdateAsync(int id, UpdateInstructorRequest request, CancellationToken ct = default);

    /// <summary>
    /// Deletes the instructor record. Throws when not found or when the instructor
    /// is still assigned to active classes.
    /// </summary>
    Task DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Bulk-imports instructor profiles from a CSV stream.
    /// Expected columns (no header required): InstructorCode, FirstName, LastName,
    /// DateOfBirth, Gender, Phone, City, Country, Email, Department, Degree.
    /// </summary>
    Task<ImportInstructorsResponse> ImportAsync(Stream csvStream, CancellationToken ct = default);
}

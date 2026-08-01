using SIMS.Application.DTOs.Subjects;

namespace SIMS.Application.Interfaces.Services;

public interface ISubjectService
{
    /// <summary>Returns all subjects. Requires the VIEW_SUB permission.</summary>
    Task<IEnumerable<SubjectResponse>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Creates a subject. Requires the CREATE_SUB permission.</summary>
    Task<SubjectResponse> CreateAsync(CreateSubjectRequest request, CancellationToken ct = default);

    /// <summary>Deletes a subject by ID. Requires the DELETE_SUB permission.</summary>
    Task DeleteAsync(int subjectId, CancellationToken ct = default);
}

using SIMS.Application.DTOs.Majors;

namespace SIMS.Application.Interfaces.Services;

public interface IMajorService
{
    /// <summary>Returns every active major.</summary>
    Task<IEnumerable<MajorResponse>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Creates a new major. Requires the CREATE_MAJOR permission.</summary>
    Task<MajorResponse> CreateAsync(CreateMajorRequest request, CancellationToken ct = default);

    /// <summary>Deletes a major by ID. Requires the DELETE_MAJOR permission.</summary>
    Task DeleteAsync(int majorId, CancellationToken ct = default);
}

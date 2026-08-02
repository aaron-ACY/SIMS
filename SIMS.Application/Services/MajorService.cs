using SIMS.Application.DTOs.Majors;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class MajorService : IMajorService
{
    private readonly IMajorRepository _majorRepository;

    public MajorService(IMajorRepository majorRepository)
    {
        _majorRepository = majorRepository;
    }

    public async Task<IEnumerable<MajorResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var majors = await _majorRepository.GetAllAsync();
        return majors.Select(MapToResponse);
    }

    public async Task<MajorResponse> CreateAsync(CreateMajorRequest request, CancellationToken ct = default)
    {
        var majorCode = request.MajorCode.Trim().ToUpperInvariant();

        if (await _majorRepository.GetByMajorCodeAsync(majorCode) is not null)
            throw new AppException(ErrorCode.MAJOR_CODE_EXISTED);

        var major = new Major
        {
            MajorCode    = majorCode,
            Name         = request.Name.Trim(),
            Description  = request.Description.Trim(),
            Department   = request.Department.Trim(),
            TotalCredits = request.TotalCredits,
            IsActive     = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _majorRepository.AddAsync(major);

        return MapToResponse(major);
    }

    public async Task DeleteAsync(int majorId, CancellationToken ct = default)
    {
        if (!await _majorRepository.DeleteAsync(majorId))
            throw new AppException(ErrorCode.MAJOR_NOT_EXISTED);
    }

    // ------------------------------------------------------------------ //

    private static MajorResponse MapToResponse(Major major) => new()
    {
        Id           = major.Id,
        MajorCode    = major.MajorCode,
        Name         = major.Name,
        Description  = major.Description,
        Department   = major.Department,
        TotalCredits = major.TotalCredits,
        IsActive     = major.IsActive
    };
}

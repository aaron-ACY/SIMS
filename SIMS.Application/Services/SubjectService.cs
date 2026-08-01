using SIMS.Application.DTOs.Subjects;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<IEnumerable<SubjectResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var subjects = await _subjectRepository.GetAllAsync();
        return subjects.Select(MapToResponse);
    }

    public async Task<SubjectResponse> CreateAsync(CreateSubjectRequest request, CancellationToken ct = default)
    {
        var subjectCode = request.SubjectCode.Trim().ToUpperInvariant();

        if (await _subjectRepository.GetBySubjectCodeAsync(subjectCode) is not null)
            throw new AppException(ErrorCode.SUBJECT_CODE_EXISTED);

        var subject = new Subject
        {
            SubjectCode  = subjectCode,
            Name         = request.Name.Trim(),
            Description  = request.Description.Trim(),
            Credits      = request.Credits,
            Department   = request.Department.Trim(),
            Major        = request.Major.Trim(),
            AcademicYear = request.AcademicYear.Trim(),
            Semester     = request.Semester,
            IsRequired   = request.IsRequired,
            IsActive     = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _subjectRepository.AddAsync(subject);

        return MapToResponse(subject);
    }

    public async Task DeleteAsync(int subjectId, CancellationToken ct = default)
    {
        if (!await _subjectRepository.DeleteAsync(subjectId))
            throw new AppException(ErrorCode.SUBJECT_NOT_EXISTED);
    }

    // ------------------------------------------------------------------ //

    private static SubjectResponse MapToResponse(Subject subject) => new()
    {
        Id           = subject.Id,
        SubjectCode  = subject.SubjectCode,
        Name         = subject.Name,
        Description  = subject.Description,
        Credits      = subject.Credits,
        Department   = subject.Department,
        Major        = subject.Major,
        AcademicYear = subject.AcademicYear,
        Semester     = subject.Semester,
        IsRequired   = subject.IsRequired,
        IsActive     = subject.IsActive
    };
}

using SIMS.Application.DTOs.Instructors;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly IUserRepository       _userRepository;
    private readonly IClassRepository      _classRepository;

    public InstructorService(
        IInstructorRepository instructorRepository,
        IUserRepository       userRepository,
        IClassRepository      classRepository)
    {
        _instructorRepository = instructorRepository;
        _userRepository       = userRepository;
        _classRepository      = classRepository;
    }

    public async Task<IEnumerable<InstructorResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var instructors = await _instructorRepository.GetAllAsync();

        // One read of users.csv, then an in-memory join — avoids a lookup per instructor.
        var users   = await _userRepository.GetAllAsync();
        var userMap = users.ToDictionary(u => u.Id);

        return instructors.Select(i => Map(i, userMap));
    }

    public async Task<InstructorResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id)
                         ?? throw new AppException(ErrorCode.INSTRUCTOR_NOT_EXISTED);

        var users   = await _userRepository.GetAllAsync();
        var userMap = users.ToDictionary(u => u.Id);

        return Map(instructor, userMap);
    }

    public async Task<InstructorResponse> CreateAsync(CreateInstructorRequest request, CancellationToken ct = default)
    {
        // Validate linked user exists.
        var user = await _userRepository.GetByIdAsync(request.UserId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        // InstructorCode must be unique (case-insensitive).
        if (await _instructorRepository.GetByInstructorCodeAsync(request.InstructorCode) is not null)
            throw new AppException(ErrorCode.INSTRUCTOR_CODE_EXISTED);

        var instructor = new Instructor
        {
            UserId         = request.UserId,
            InstructorCode = request.InstructorCode.Trim().ToUpperInvariant(),
            Department     = request.Department.Trim(),
            Degree         = request.Degree.Trim(),
            Specialization = request.Specialization.Trim(),
            HireDate       = request.HireDate,
            Phone          = request.Phone.Trim(),
            IsActive       = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _instructorRepository.AddAsync(instructor);

        return Map(instructor, new Dictionary<int, User> { [user.Id] = user });
    }

    public async Task<InstructorResponse> UpdateAsync(int id, UpdateInstructorRequest request, CancellationToken ct = default)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id)
                         ?? throw new AppException(ErrorCode.INSTRUCTOR_NOT_EXISTED);

        // If changing InstructorCode, ensure it stays unique.
        if (request.InstructorCode is not null)
        {
            var conflict = await _instructorRepository.GetByInstructorCodeAsync(request.InstructorCode);
            if (conflict is not null && conflict.Id != id)
                throw new AppException(ErrorCode.INSTRUCTOR_CODE_EXISTED);

            instructor.InstructorCode = request.InstructorCode.Trim().ToUpperInvariant();
        }

        if (request.Department     is not null) instructor.Department     = request.Department.Trim();
        if (request.Degree         is not null) instructor.Degree         = request.Degree.Trim();
        if (request.Specialization is not null) instructor.Specialization = request.Specialization.Trim();
        if (request.HireDate.HasValue)          instructor.HireDate       = request.HireDate.Value;
        if (request.Phone          is not null) instructor.Phone          = request.Phone.Trim();

        instructor.UpdatedAt = DateTime.UtcNow;
        await _instructorRepository.UpdateAsync(instructor);

        var user    = await _userRepository.GetByIdAsync(instructor.UserId);
        var userMap = user is null
            ? new Dictionary<int, User>()
            : new Dictionary<int, User> { [user.Id] = user };

        return Map(instructor, userMap);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        // Guard: refuse deletion if the instructor is assigned to active classes.
        var activeClasses = (await _classRepository.GetAllAsync())
            .Any(c => c.InstructorId == id && c.IsActive);
        if (activeClasses)
            throw new AppException(ErrorCode.USER_HAS_ACTIVE_CLASSES);

        if (!await _instructorRepository.DeleteAsync(id))
            throw new AppException(ErrorCode.INSTRUCTOR_NOT_EXISTED);
    }

    // ------------------------------------------------------------------ //

    private static InstructorResponse Map(
        Instructor instructor,
        Dictionary<int, User> userMap)
    {
        userMap.TryGetValue(instructor.UserId, out var user);

        return new InstructorResponse
        {
            InstructorCode = instructor.InstructorCode,
            FullName       = user?.FullName       ?? string.Empty,
            Email          = user?.Email          ?? string.Empty,
            Department     = instructor.Department,
            Degree         = instructor.Degree,
            Specialization = instructor.Specialization,
            HireDate       = instructor.HireDate,
            Phone          = instructor.Phone,
            IsActive       = instructor.IsActive
        };
    }
}

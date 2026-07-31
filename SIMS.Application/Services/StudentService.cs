using SIMS.Application.DTOs.Students;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;

    public StudentService(
        IStudentRepository studentRepository,
        IUserRepository userRepository)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<StudentResponse>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        var users    = await _userRepository.GetAllAsync();
        var userMap  = users.ToDictionary(u => u.Id);

        return students.Select(s => Map(s, userMap));
    }

    public async Task<StudentResponse> GetByIdAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id)
                      ?? throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        var users   = await _userRepository.GetAllAsync();
        var userMap = users.ToDictionary(u => u.Id);

        return Map(student, userMap);
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request)
    {
        // Validate linked user exists
        var user = await _userRepository.GetByIdAsync(request.UserId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        // StudentCode must be unique (case-insensitive)
        var existing = await _studentRepository.GetByStudentCodeAsync(request.StudentCode);
        if (existing is not null)
            throw new AppException(ErrorCode.STUDENT_CODE_EXISTED);

        var student = new Student
        {
            UserId         = request.UserId,
            StudentCode    = request.StudentCode.ToUpperInvariant(),
            DateOfBirth    = request.DateOfBirth,
            Gender         = request.Gender.Trim(),
            Phone          = request.Phone.Trim(),
            Address        = request.Address.Trim(),
            Major          = request.Major.Trim(),
            EnrollmentYear = request.EnrollmentYear,
            Status         = request.Status.Trim(),
            IsActive       = true
        };

        await _studentRepository.AddAsync(student);

        return Map(student, new Dictionary<int, Domain.Entities.User> { [user.Id] = user });
    }

    public async Task<StudentResponse> UpdateAsync(int id, UpdateStudentRequest request)
    {
        var student = await _studentRepository.GetByIdAsync(id)
                      ?? throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        // If changing StudentCode, ensure it stays unique
        if (request.StudentCode is not null)
        {
            var conflict = await _studentRepository.GetByStudentCodeAsync(request.StudentCode);
            if (conflict is not null && conflict.Id != id)
                throw new AppException(ErrorCode.STUDENT_CODE_EXISTED);

            student.StudentCode = request.StudentCode.ToUpperInvariant();
        }

        if (request.DateOfBirth.HasValue)    student.DateOfBirth    = request.DateOfBirth.Value;
        if (request.Gender     is not null)  student.Gender         = request.Gender.Trim();
        if (request.Phone      is not null)  student.Phone          = request.Phone.Trim();
        if (request.Address    is not null)  student.Address        = request.Address.Trim();
        if (request.Major      is not null)  student.Major          = request.Major.Trim();
        if (request.EnrollmentYear.HasValue) student.EnrollmentYear = request.EnrollmentYear.Value;
        if (request.Status     is not null)  student.Status         = request.Status.Trim();

        await _studentRepository.UpdateAsync(student);

        var user    = await _userRepository.GetByIdAsync(student.UserId);
        var userMap = user is null
            ? new Dictionary<int, Domain.Entities.User>()
            : new Dictionary<int, Domain.Entities.User> { [user.Id] = user };

        return Map(student, userMap);
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _studentRepository.DeleteAsync(id);
        if (!deleted)
            throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);
    }

    // ------------------------------------------------------------------ //

    private static StudentResponse Map(
        Student student,
        Dictionary<int, Domain.Entities.User> userMap)
    {
        userMap.TryGetValue(student.UserId, out var user);

        return new StudentResponse
        {
            StudentCode    = student.StudentCode,
            FullName       = user?.FullName ?? string.Empty,
            Email          = user?.Email    ?? string.Empty,
            DateOfBirth    = student.DateOfBirth,
            Gender         = student.Gender,
            Phone          = student.Phone,
            Address        = student.Address,
            Major          = student.Major,
            EnrollmentYear = student.EnrollmentYear,
            Status         = student.Status,
            IsActive       = student.IsActive
        };
    }
}

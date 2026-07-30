using SIMS.Application.DTOs.Grades;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository      _gradeRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository    _studentRepository;
    private readonly IClassRepository      _classRepository;
    private readonly IUserRepository       _userRepository;
    private readonly IRoleRepository       _roleRepository;

    public GradeService(
        IGradeRepository      gradeRepository,
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository    studentRepository,
        IClassRepository      classRepository,
        IUserRepository       userRepository,
        IRoleRepository       roleRepository)
    {
        _gradeRepository      = gradeRepository;
        _enrollmentRepository = enrollmentRepository;
        _studentRepository    = studentRepository;
        _classRepository      = classRepository;
        _userRepository       = userRepository;
        _roleRepository       = roleRepository;
    }

    public async Task<GradeResponse> CreateAsync(CreateGradeRequest request)
    {
        // Verify the enrollment exists and is active.
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId)
                         ?? throw new AppException(ErrorCode.ENROLLMENT_NOT_EXISTED);

        // Prevent duplicate grade for the same enrollment.
        if (await _gradeRepository.GetByEnrollmentIdAsync(enrollment.Id) is not null)
            throw new AppException(ErrorCode.GRADE_ALREADY_EXISTS);

        var grade = new Grade
        {
            EnrollmentId   = enrollment.Id,
            StudentId      = enrollment.StudentId,
            ClassId        = enrollment.ClassId,
            Score          = request.Score,
            Classification = Classify(request.Score),
            GradedAt       = DateTime.UtcNow,
            UpdatedAt      = DateTime.UtcNow
        };

        await _gradeRepository.AddAsync(grade);

        return await BuildResponseAsync(grade);
    }

    public async Task<GradeResponse> UpdateAsync(int gradeId, UpdateGradeRequest request)
    {
        var grade = await _gradeRepository.GetByIdAsync(gradeId)
                    ?? throw new AppException(ErrorCode.GRADE_NOT_EXISTED);

        grade.Score          = request.Score;
        grade.Classification = Classify(request.Score);
        grade.UpdatedAt      = DateTime.UtcNow;

        await _gradeRepository.UpdateAsync(grade);

        return await BuildResponseAsync(grade);
    }

    public async Task<IEnumerable<GradeResponse>> GetScoresByUserIdAsync(int userId)
    {
        // Verify the user exists.
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new AppException(ErrorCode.USER_NOT_EXISTED);

        // The endpoint is only valid for student accounts.
        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        if (role?.Name != Roles.Student)
            throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        // Resolve the student record linked to this user account.
        var student = await _studentRepository.GetByUserIdAsync(userId)
                      ?? throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        var grades = await _gradeRepository.GetByStudentIdAsync(student.Id);

        var responses = new List<GradeResponse>();
        foreach (var grade in grades)
            responses.Add(await BuildResponseAsync(grade));

        return responses;
    }

    // ------------------------------------------------------------------ //

    /// <summary>
    /// Maps a numeric score to its classification label.
    /// Refer &lt;6.5 | Pass 6.5–7.9 | Merit 8–8.9 | Distinction 9–10.
    /// </summary>
    public static string Classify(double score) => score switch
    {
        >= 9.0              => "Distinction",
        >= 8.0              => "Merit",
        >= 6.5              => "Pass",
        _                   => "Refer"
    };

    private async Task<GradeResponse> BuildResponseAsync(Grade grade)
    {
        var student    = await _studentRepository.GetByIdAsync(grade.StudentId);
        var schoolClass = await _classRepository.GetByIdAsync(grade.ClassId);

        // Resolve student's display name via their linked user account.
        string studentName = string.Empty;
        if (student is not null)
        {
            var user = await _userRepository.GetByIdAsync(student.UserId);
            studentName = user?.FullName ?? string.Empty;
        }

        return new GradeResponse
        {
            Id             = grade.Id,
            EnrollmentId   = grade.EnrollmentId,
            StudentId      = grade.StudentId,
            StudentName    = studentName,
            ClassId        = grade.ClassId,
            ClassCode      = schoolClass?.ClassCode ?? string.Empty,
            Score          = grade.Score,
            Classification = grade.Classification,
            GradedAt       = grade.GradedAt,
            UpdatedAt      = grade.UpdatedAt
        };
    }
}

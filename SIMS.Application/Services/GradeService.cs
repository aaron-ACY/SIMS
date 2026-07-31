using SIMS.Application.DTOs.Grades;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository      _gradeRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository    _studentRepository;
    private readonly IClassRepository      _classRepository;
    private readonly ISubjectRepository    _subjectRepository;
    private readonly IUserRepository       _userRepository;

    public GradeService(
        IGradeRepository      gradeRepository,
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository    studentRepository,
        IClassRepository      classRepository,
        ISubjectRepository    subjectRepository,
        IUserRepository       userRepository)
    {
        _gradeRepository      = gradeRepository;
        _enrollmentRepository = enrollmentRepository;
        _studentRepository    = studentRepository;
        _classRepository      = classRepository;
        _subjectRepository    = subjectRepository;
        _userRepository       = userRepository;
    }

    public async Task<GradeResponse> CreateAsync(CreateGradeRequest request)
    {
        // Verify the enrollment exists.
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

    public async Task<StudentGradeReportResponse> GetScoresByStudentCodeAsync(string studentCode)
    {
        var student = await _studentRepository.GetByStudentCodeAsync(studentCode)
                      ?? throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        var user = await _userRepository.GetByIdAsync(student.UserId);

        var grades = (await _gradeRepository.GetByStudentIdAsync(student.Id))
                     .OrderByDescending(g => g.GradedAt)
                     .ToList();

        // Top-level class/semester: taken from the most-recent grade's class.
        string classCode   = string.Empty;
        int    semester    = 0;
        bool   topLevelSet = false;

        var gradeItems = new List<GradeItemResponse>();

        foreach (var grade in grades)
        {
            var schoolClass = await _classRepository.GetByIdAsync(grade.ClassId);
            var subject     = schoolClass is not null
                              ? await _subjectRepository.GetByIdAsync(schoolClass.SubjectId)
                              : null;

            if (!topLevelSet && schoolClass is not null)
            {
                classCode   = schoolClass.ClassCode;
                semester    = schoolClass.Semester;
                topLevelSet = true;
            }

            gradeItems.Add(new GradeItemResponse
            {
                SubjectCode = subject?.SubjectCode ?? string.Empty,
                SubjectName = subject?.Name        ?? string.Empty,
                Scores      = grade.Score,
                Rating      = grade.Classification
            });
        }

        return new StudentGradeReportResponse
        {
            StudentCode = student.StudentCode,
            FirstName   = user?.FirstName ?? string.Empty,
            LastName    = user?.LastName  ?? string.Empty,
            Class       = classCode,
            Semester    = semester,
            Grades      = gradeItems
        };
    }

    // ------------------------------------------------------------------ //

    /// <summary>
    /// Maps a numeric score to its classification label.
    /// Refer &lt;6.5 | Pass 6.5–7.9 | Merit 8–8.9 | Distinction 9–10.
    /// </summary>
    public static string Classify(double score) => score switch
    {
        >= 9.0 => "Distinction",
        >= 8.0 => "Merit",
        >= 6.5 => "Pass",
        _      => "Refer"
    };

    private async Task<GradeResponse> BuildResponseAsync(Grade grade)
    {
        var student     = await _studentRepository.GetByIdAsync(grade.StudentId);
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

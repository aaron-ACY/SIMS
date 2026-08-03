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

    public async Task<GradeResponse> SubmitAsync(int enrollmentId, string submissionPath, CancellationToken ct = default)
    {
        // Verify the enrollment exists.
        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId)
                         ?? throw new AppException(ErrorCode.ENROLLMENT_NOT_EXISTED);

        var existing = await _gradeRepository.GetByEnrollmentIdAsync(enrollmentId);

        Grade grade;
        if (existing is null)
        {
            // First submission — create the grade record without a score yet.
            grade = new Grade
            {
                EnrollmentId   = enrollment.Id,
                StudentId      = enrollment.StudentId,
                ClassId        = enrollment.ClassId,
                SubmissionPath = submissionPath,
                UpdatedAt      = DateTime.UtcNow
            };
            await _gradeRepository.AddAsync(grade);
        }
        else
        {
            // Re-submission — overwrite the path; preserve any existing score.
            existing.SubmissionPath = submissionPath;
            existing.UpdatedAt      = DateTime.UtcNow;
            await _gradeRepository.UpdateAsync(existing);
            grade = existing;
        }

        return await BuildResponseAsync(grade);
    }

    public async Task<GradeResponse> CreateAsync(CreateGradeRequest request, CancellationToken ct = default)
    {
        // Verify the enrollment exists.
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId)
                         ?? throw new AppException(ErrorCode.ENROLLMENT_NOT_EXISTED);

        // A grade can only be entered after the student has submitted their assignment.
        var grade = await _gradeRepository.GetByEnrollmentIdAsync(enrollment.Id);

        if (grade is null || grade.SubmissionPath is null)
            throw new AppException(ErrorCode.SUBMISSION_NOT_FOUND);

        // Prevent entering a grade twice for the same enrollment.
        if (grade.GradedAt is not null)
            throw new AppException(ErrorCode.GRADE_ALREADY_EXISTS);

        grade.Score          = request.Score;
        grade.Classification = Classify(request.Score);
        grade.GradedAt       = DateTime.UtcNow;
        grade.UpdatedAt      = DateTime.UtcNow;

        await _gradeRepository.UpdateAsync(grade);

        return await BuildResponseAsync(grade);
    }

    public async Task<GradeResponse> UpdateAsync(int gradeId, UpdateGradeRequest request, CancellationToken ct = default)
    {
        var grade = await _gradeRepository.GetByIdAsync(gradeId)
                    ?? throw new AppException(ErrorCode.GRADE_NOT_EXISTED);

        // Can only edit a grade that has already been formally entered.
        if (grade.GradedAt is null)
            throw new AppException(ErrorCode.GRADE_NOT_YET_ENTERED);

        grade.Score          = request.Score;
        grade.Classification = Classify(request.Score);
        grade.UpdatedAt      = DateTime.UtcNow;

        await _gradeRepository.UpdateAsync(grade);

        return await BuildResponseAsync(grade);
    }

    public async Task<StudentGradeReportResponse> GetScoresByStudentCodeAsync(string studentCode, CancellationToken ct = default)
    {
        var student = await _studentRepository.GetByStudentCodeAsync(studentCode)
                      ?? throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        var user = await _userRepository.GetByIdAsync(student.UserId ?? 0);

        var grades = (await _gradeRepository.GetByStudentIdAsync(student.Id))
                     .OrderByDescending(g => g.GradedAt)
                     .ToList();

        // Load all classes and subjects once, then use dictionary lookup
        // inside the loop — avoids 2N CSV reads (one per grade) and
        // replaces them with 2 reads total.
        var classMap   = (await _classRepository.GetAllAsync())
                             .ToDictionary(c => c.Id);
        var subjectMap = (await _subjectRepository.GetAllAsync())
                             .ToDictionary(s => s.Id);

        // Top-level class/semester: taken from the most-recent grade's class.
        string classCode   = string.Empty;
        int    semester    = 0;
        bool   topLevelSet = false;

        var gradeItems = new List<GradeItemResponse>();

        foreach (var grade in grades)
        {
            classMap.TryGetValue(grade.ClassId, out var schoolClass);
            var subject = schoolClass is not null &&
                          subjectMap.TryGetValue(schoolClass.SubjectId, out var s)
                          ? s : null;

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

    public async Task<IEnumerable<GradeResponse>> GetGradesByClassIdAsync(int classId, CancellationToken ct = default)
    {
        if (await _classRepository.GetByIdAsync(classId) is null)
            throw new AppException(ErrorCode.CLASS_NOT_EXISTED);

        var grades = (await _gradeRepository.GetByClassIdAsync(classId)).ToList();

        var results = new List<GradeResponse>(grades.Count);
        foreach (var grade in grades)
            results.Add(await BuildResponseAsync(grade));

        return results;
    }

    // ------------------------------------------------------------------ //

    private async Task<GradeResponse> BuildResponseAsync(Grade grade)
    {
        var student     = await _studentRepository.GetByIdAsync(grade.StudentId);
        var schoolClass = await _classRepository.GetByIdAsync(grade.ClassId);

        // Resolve student's display name via their linked user account.
        string studentName = string.Empty;
        if (student is not null)
        {
            var user = await _userRepository.GetByIdAsync(student.UserId ?? 0);
            studentName = user?.FullName ?? string.Empty;
        }

        return new GradeResponse
        {
            Id             = grade.Id,
            EnrollmentId   = grade.EnrollmentId,
            StudentId      = grade.StudentId,
            StudentCode    = student?.StudentCode ?? string.Empty,
            StudentName    = studentName,
            ClassId        = grade.ClassId,
            ClassCode      = schoolClass?.ClassCode ?? string.Empty,
            Score          = grade.Score,
            Classification = grade.Classification,
            SubmissionPath = grade.SubmissionPath,
            GradedAt       = grade.GradedAt,
            UpdatedAt      = grade.UpdatedAt
        };
    }
}

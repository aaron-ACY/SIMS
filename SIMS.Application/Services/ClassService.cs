using SIMS.Application.DTOs.Classes;
using SIMS.Application.DTOs.Enrollments;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository      _classRepository;
    private readonly ISubjectRepository    _subjectRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IUserRepository       _userRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository    _studentRepository;

    public ClassService(
        IClassRepository      classRepository,
        ISubjectRepository    subjectRepository,
        IInstructorRepository instructorRepository,
        IUserRepository       userRepository,
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository    studentRepository)
    {
        _classRepository      = classRepository;
        _subjectRepository    = subjectRepository;
        _instructorRepository = instructorRepository;
        _userRepository       = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _studentRepository    = studentRepository;
    }

    public async Task<ClassResponse> CreateAsync(CreateClassRequest request)
    {
        var classCode = request.ClassCode.Trim().ToUpperInvariant();

        if (await _classRepository.GetByClassCodeAsync(classCode) is not null)
            throw new AppException(ErrorCode.CLASS_CODE_EXISTED);

        var subject = await _subjectRepository.GetByIdAsync(request.SubjectId)
                      ?? throw new AppException(ErrorCode.SUBJECT_NOT_EXISTED);

        var instructor = await _instructorRepository.GetByIdAsync(request.InstructorId)
                         ?? throw new AppException(ErrorCode.INSTRUCTOR_NOT_EXISTED);

        var schoolClass = new Class
        {
            ClassCode        = classCode,
            SubjectId        = subject.Id,
            InstructorId     = instructor.Id,
            Semester         = request.Semester,
            AcademicYear     = request.AcademicYear.Trim(),
            Room             = request.Room.Trim(),
            Schedule         = request.Schedule.Trim(),
            MaxEnrollment    = request.MaxEnrollment,
            CurrentEnrollment = 0,
            IsActive         = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _classRepository.AddAsync(schoolClass);

        var user = await _userRepository.GetByIdAsync(instructor.UserId);
        return MapToResponse(schoolClass, subject.Name, user?.FullName ?? string.Empty);
    }

    public async Task EnrollStudentAsync(int classId, EnrollStudentRequest request)
    {
        var schoolClass = await _classRepository.GetByIdAsync(classId)
                          ?? throw new AppException(ErrorCode.CLASS_NOT_EXISTED);

        if (!schoolClass.IsActive)
            throw new AppException(ErrorCode.CLASS_NOT_EXISTED);

        if (schoolClass.CurrentEnrollment >= schoolClass.MaxEnrollment)
            throw new AppException(ErrorCode.CLASS_FULL);

        var student = await _studentRepository.GetByIdAsync(request.StudentId)
                      ?? throw new AppException(ErrorCode.STUDENT_NOT_EXISTED);

        if (await _enrollmentRepository.GetAsync(classId, student.Id) is not null)
            throw new AppException(ErrorCode.ALREADY_ENROLLED);

        var enrollment = new Enrollment
        {
            ClassId    = classId,
            StudentId  = student.Id,
            EnrolledAt = DateTime.UtcNow,
            IsActive   = true
        };

        await _enrollmentRepository.AddAsync(enrollment);
        await _classRepository.UpdateEnrollmentCountAsync(classId, +1);
    }

    public async Task RemoveStudentAsync(int classId, int studentId)
    {
        if (await _classRepository.GetByIdAsync(classId) is null)
            throw new AppException(ErrorCode.CLASS_NOT_EXISTED);

        if (!await _enrollmentRepository.DeleteAsync(classId, studentId))
            throw new AppException(ErrorCode.ENROLLMENT_NOT_EXISTED);

        await _classRepository.UpdateEnrollmentCountAsync(classId, -1);
    }

    // ------------------------------------------------------------------ //

    private static ClassResponse MapToResponse(
        Class schoolClass, string subjectName, string instructorName) => new()
    {
        Id                = schoolClass.Id,
        ClassCode         = schoolClass.ClassCode,
        SubjectId         = schoolClass.SubjectId,
        SubjectName       = subjectName,
        InstructorId      = schoolClass.InstructorId,
        InstructorName    = instructorName,
        Semester          = schoolClass.Semester,
        AcademicYear      = schoolClass.AcademicYear,
        Room              = schoolClass.Room,
        Schedule          = schoolClass.Schedule,
        MaxEnrollment     = schoolClass.MaxEnrollment,
        CurrentEnrollment = schoolClass.CurrentEnrollment,
        IsActive          = schoolClass.IsActive
    };
}

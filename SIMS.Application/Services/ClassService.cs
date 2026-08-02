using SIMS.Application.DTOs.Classes;
using SIMS.Application.DTOs.Enrollments;
using SIMS.Application.DTOs.Students;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository          _classRepository;
    private readonly ISubjectRepository        _subjectRepository;
    private readonly IInstructorRepository     _instructorRepository;
    private readonly IUserRepository           _userRepository;
    private readonly IEnrollmentRepository     _enrollmentRepository;
    private readonly IStudentRepository        _studentRepository;
    private readonly EnrollmentSemaphoreService _enrollmentLock;

    public ClassService(
        IClassRepository          classRepository,
        ISubjectRepository        subjectRepository,
        IInstructorRepository     instructorRepository,
        IUserRepository           userRepository,
        IEnrollmentRepository     enrollmentRepository,
        IStudentRepository        studentRepository,
        EnrollmentSemaphoreService enrollmentLock)
    {
        _classRepository      = classRepository;
        _subjectRepository    = subjectRepository;
        _instructorRepository = instructorRepository;
        _userRepository       = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _studentRepository    = studentRepository;
        _enrollmentLock       = enrollmentLock;
    }

    public async Task<IEnumerable<ClassListItemResponse>> GetClassesAsync(CancellationToken ct = default)
    {
        var classes     = await _classRepository.GetAllAsync();
        var subjects    = await _subjectRepository.GetAllAsync();
        var instructors = await _instructorRepository.GetAllAsync();
        var users       = await _userRepository.GetAllAsync();

        var subjectMap    = subjects.ToDictionary(s => s.Id, s => s.Name);
        var instructorMap = instructors.ToDictionary(i => i.Id);
        var userMap       = users.ToDictionary(u => u.Id, u => u.FullName);

        return classes.Select(c =>
        {
            subjectMap.TryGetValue(c.SubjectId, out var subjectName);
            instructorMap.TryGetValue(c.InstructorId, out var instructor);
            var instructorName = instructor is not null && instructor.UserId.HasValue && userMap.TryGetValue(instructor.UserId.Value, out var n)
                ? n : string.Empty;

            return new ClassListItemResponse
            {
                Id                = c.Id,
                ClassCode         = c.ClassCode,
                SubjectName       = subjectName ?? string.Empty,
                InstructorName    = instructorName,
                MaxEnrollment     = c.MaxEnrollment,
                CurrentEnrollment = c.CurrentEnrollment,
                IsActive          = c.IsActive
            };
        });
    }

    public async Task<ClassEnrollmentsResponse> GetStudentsInClassAsync(int classId, CancellationToken ct = default)
    {
        var schoolClass = await _classRepository.GetByIdAsync(classId)
                          ?? throw new AppException(ErrorCode.CLASS_NOT_EXISTED);

        var enrollments = await _enrollmentRepository.GetByClassIdAsync(classId);
        var studentIds  = enrollments.Select(e => e.StudentId).ToHashSet();

        var allStudents = await _studentRepository.GetAllAsync();
        var studentMap  = allStudents.Where(s => studentIds.Contains(s.Id))
                                     .ToDictionary(s => s.Id);

        var users   = await _userRepository.GetAllAsync();
        var userMap = users.ToDictionary(u => u.Id);

        var enrollmentItems = enrollments.Select(e =>
        {
            studentMap.TryGetValue(e.StudentId, out var student);
            userMap.TryGetValue(student?.UserId ?? 0, out var user);

            return new EnrollmentItemResponse
            {
                EnrollmentId = e.Id,
                Student = new EnrollmentStudentInfo
                {
                    StudentCode = student?.StudentCode ?? string.Empty,
                    FullName    = user?.FullName ?? string.Empty,
                    DateOfBirth = student?.DateOfBirth ?? default,
                    Gender      = student?.Gender ?? string.Empty
                },
                Status     = student?.Status ?? string.Empty,
                EnrolledAt = e.EnrolledAt
            };
        }).ToList();

        return new ClassEnrollmentsResponse
        {
            ClassCode     = schoolClass.ClassCode,
            SchoolYear    = schoolClass.AcademicYear,
            TotalStudents = enrollmentItems.Count,
            Enrollments   = enrollmentItems
        };
    }

    public async Task<ClassResponse> CreateAsync(CreateClassRequest request, CancellationToken ct = default)
    {
        var classCode = (request.ClassCode?.Trim() ?? string.Empty).ToUpperInvariant();

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
            AcademicYear     = request.AcademicYear?.Trim() ?? string.Empty,
            Room             = request.Room?.Trim()         ?? string.Empty,
            Schedule         = request.Schedule?.Trim()     ?? string.Empty,
            MaxEnrollment    = request.MaxEnrollment,
            CurrentEnrollment = 0,
            IsActive         = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _classRepository.AddAsync(schoolClass);

        var user = await _userRepository.GetByIdAsync(instructor.UserId ?? 0);
        return MapToResponse(schoolClass, subject.Name, user?.FullName ?? string.Empty);
    }

    public async Task EnrollStudentAsync(int classId, EnrollStudentRequest request, CancellationToken ct = default)
    {
        // Acquire a per-class lock so the capacity-check → add-enrollment →
        // increment-count sequence is atomic with respect to other concurrent
        // enrolment requests for the same class.
        var sem = _enrollmentLock.GetSemaphore(classId);
        await sem.WaitAsync();
        try
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

            // UpdateEnrollmentCountAsync also re-checks capacity inside its own
            // repository lock (defence-in-depth). Returns false only if the class
            // disappeared — not expected here since we already hold the lock.
            await _classRepository.UpdateEnrollmentCountAsync(classId, +1);
        }
        finally
        {
            sem.Release();
        }
    }

    public async Task RemoveStudentAsync(int classId, int studentId, CancellationToken ct = default)
    {
        // Serialise removal for the same class so the delete-enrollment →
        // decrement-count pair is not interleaved with a concurrent enrolment.
        var sem = _enrollmentLock.GetSemaphore(classId);
        await sem.WaitAsync();
        try
        {
            if (await _classRepository.GetByIdAsync(classId) is null)
                throw new AppException(ErrorCode.CLASS_NOT_EXISTED);

            if (!await _enrollmentRepository.DeleteAsync(classId, studentId))
                throw new AppException(ErrorCode.ENROLLMENT_NOT_EXISTED);

            await _classRepository.UpdateEnrollmentCountAsync(classId, -1);
        }
        finally
        {
            sem.Release();
        }
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

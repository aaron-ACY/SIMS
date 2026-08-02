using SIMS.Application.DTOs.Courses;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IUserRepository _userRepository;

    public CourseService(
        ICourseRepository courseRepository,
        IInstructorRepository instructorRepository,
        IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _instructorRepository = instructorRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<CourseResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var courses = await _courseRepository.GetAllAsync();

        // Course → Instructor → User, resolved with one read of each file.
        var instructors = await _instructorRepository.GetAllAsync();
        var users       = await _userRepository.GetAllAsync();

        var userMap = users.ToDictionary(u => u.Id);
        var instructorNameMap = instructors.ToDictionary(
            i => i.Id,
            i => i.UserId.HasValue && userMap.TryGetValue(i.UserId.Value, out var u) ? u.FullName : string.Empty);

        return courses.Select(c => MapToResponse(
            c,
            instructorNameMap.TryGetValue(c.InstructorId, out var name) ? name : string.Empty));
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken ct = default)
    {
        var courseCode = request.CourseCode.Trim().ToUpperInvariant();

        if (await _courseRepository.GetByCourseCodeAsync(courseCode) is not null)
            throw new AppException(ErrorCode.COURSE_CODE_EXISTED);

        // The course must point at a real instructor, otherwise the listing endpoint
        // would return a course with a blank InstructorName.
        var instructor = await _instructorRepository.GetByIdAsync(request.InstructorId)
                         ?? throw new AppException(ErrorCode.INSTRUCTOR_NOT_EXISTED);

        var course = new Course
        {
            CourseCode    = courseCode,
            Name          = request.Name.Trim(),
            Description   = request.Description.Trim(),
            Credits       = request.Credits,
            InstructorId  = instructor.Id,
            Semester      = request.Semester,
            AcademicYear  = request.AcademicYear.Trim(),
            MaxEnrollment = request.MaxEnrollment,
            IsActive      = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _courseRepository.AddAsync(course);

        var user = await _userRepository.GetByIdAsync(instructor.UserId ?? 0);
        return MapToResponse(course, user?.FullName ?? string.Empty);
    }

    public async Task DeleteAsync(int courseId, CancellationToken ct = default)
    {
        if (!await _courseRepository.DeleteAsync(courseId))
            throw new AppException(ErrorCode.COURSE_NOT_EXISTED);
    }

    // ------------------------------------------------------------------ //

    private static CourseResponse MapToResponse(Course course, string instructorName) => new()
    {
        Id             = course.Id,
        CourseCode     = course.CourseCode,
        Name           = course.Name,
        Description    = course.Description,
        Credits        = course.Credits,
        InstructorId   = course.InstructorId,
        InstructorName = instructorName,
        Semester       = course.Semester,
        AcademicYear   = course.AcademicYear,
        MaxEnrollment  = course.MaxEnrollment,
        IsActive       = course.IsActive
    };
}

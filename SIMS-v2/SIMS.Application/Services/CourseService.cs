using SIMS.Application.DTOs.Courses;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<CourseResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var courses = await _courseRepository.GetAllAsync();
        return courses.Select(MapToResponse);
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken ct = default)
    {
        var courseCode = (request.CourseCode?.Trim() ?? string.Empty).ToUpperInvariant();

        if (await _courseRepository.GetByCourseCodeAsync(courseCode) is not null)
            throw new AppException(ErrorCode.COURSE_CODE_EXISTED);

        var course = new Course
        {
            CourseCode  = courseCode,
            Name        = request.Name.Trim(),
            Description = request.Description.Trim(),
            Credits     = request.Credits,
            IsRequired  = request.IsRequired,
            IsActive    = true
        };

        // AddAsync assigns Id, CreatedAt and UpdatedAt.
        await _courseRepository.AddAsync(course);

        return MapToResponse(course);
    }

    public async Task DeleteAsync(int courseId, CancellationToken ct = default)
    {
        if (!await _courseRepository.DeleteAsync(courseId))
            throw new AppException(ErrorCode.COURSE_NOT_EXISTED);
    }

    // ------------------------------------------------------------------ //

    private static CourseResponse MapToResponse(Course course) => new()
    {
        Id          = course.Id,
        CourseCode  = course.CourseCode,
        Name        = course.Name,
        Description = course.Description,
        Credits     = course.Credits,
        IsRequired  = course.IsRequired,
        IsActive    = course.IsActive
    };
}

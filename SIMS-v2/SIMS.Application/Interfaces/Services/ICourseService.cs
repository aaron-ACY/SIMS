using SIMS.Application.DTOs.Courses;

namespace SIMS.Application.Interfaces.Services;

public interface ICourseService
{
    /// <summary>Returns every course (môn học).</summary>
    Task<IEnumerable<CourseResponse>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Creates a course. Requires the CREATE_COURSE permission.</summary>
    Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken ct = default);

    /// <summary>Deletes a course by ID. Requires the DELETE_COURSE permission.</summary>
    Task DeleteAsync(int courseId, CancellationToken ct = default);
}

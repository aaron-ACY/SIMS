using SIMS.Application.DTOs.Courses;

namespace SIMS.Application.Interfaces.Services;

public interface ICourseService
{
    /// <summary>
    /// Returns every course with the instructor's name resolved.
    /// Requires the VIEW_COURSES permission.
    /// </summary>
    Task<IEnumerable<CourseResponse>> GetAllAsync();

    /// <summary>
    /// Creates a course. Requires the CREATE_COURSE permission.
    /// </summary>
    Task<CourseResponse> CreateAsync(CreateCourseRequest request);

    /// <summary>
    /// Deletes a course by ID. Requires the DELETE_COURSE permission.
    /// </summary>
    Task DeleteAsync(int courseId);
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Courses;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/majors")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Returns all courses with the teaching instructor's name resolved.
    /// Requires the VIEW_MAJOR permission.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewCourses)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CourseResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllCourses(CancellationToken ct)
    {
        var courses = await _courseService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<CourseResponse>>.Ok(courses));
    }

    /// <summary>
    /// Creates a new course. Requires the CREATE_MAJOR permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateCourse)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request, CancellationToken ct)
    {
        var course = await _courseService.CreateAsync(request, ct);

        // No GET /api/courses/{id} endpoint exists to serve as the Location target,
        // so a bare 201 is returned instead of a Location header pointing nowhere.
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<CourseResponse>.Ok(course, "Course created successfully."));
    }

    /// <summary>
    /// Deletes a course by ID. Requires the DELETE_MAJOR permission.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.DeleteCourse)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(int id, CancellationToken ct)
    {
        await _courseService.DeleteAsync(id, ct);
        return Ok(ApiResponse.Ok("Course deleted successfully."));
    }
}

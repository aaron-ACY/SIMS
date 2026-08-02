using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Classes;
using SIMS.Application.DTOs.Enrollments;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/classes")]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;

    public ClassesController(IClassService classService)
    {
        _classService = classService;
    }

    /// <summary>
    /// Returns all classes. Requires the VIEW_CLA permission.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewClasses)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClassListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetClasses(CancellationToken ct)
    {
        var classes = await _classService.GetClassesAsync(ct);
        return Ok(ApiResponse<IEnumerable<ClassListItemResponse>>.Ok(classes));
    }

    /// <summary>
    /// Returns class info with enrolled students. Requires the LIST_STU permission.
    /// </summary>
    [HttpGet("{classId:int}/enrollments")]
    [Authorize(Policy = Permissions.ListStudentsInClass)]
    [ProducesResponseType(typeof(ApiResponse<ClassEnrollmentsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentsInClass(int classId, CancellationToken ct)
    {
        var response = await _classService.GetStudentsInClassAsync(classId, ct);
        return Ok(ApiResponse<ClassEnrollmentsResponse>.Ok(response));
    }

    /// <summary>
    /// Creates a new class. Requires the CREATE_CLASS permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateClass)]
    [ProducesResponseType(typeof(ApiResponse<ClassResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request, CancellationToken ct)
    {
        var schoolClass = await _classService.CreateAsync(request, ct);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<ClassResponse>.Ok(schoolClass, "Class created successfully."));
    }

    /// <summary>
    /// Enrolls a student into a class. Requires the ENROLLMENTS permission.
    /// </summary>
    [HttpPost("{classId:int}/enrollments")]
    [Authorize(Policy = Permissions.Enrollments)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> EnrollStudent(
        int classId, [FromBody] EnrollStudentRequest request, CancellationToken ct)
    {
        await _classService.EnrollStudentAsync(classId, request, ct);
        return Ok(ApiResponse.Ok("Student enrolled successfully."));
    }

    /// <summary>
    /// Removes a student from a class. Requires the GETOUT permission.
    /// </summary>
    [HttpDelete("{classId:int}/enrollments/{studentId:int}")]
    [Authorize(Policy = Permissions.GetOut)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveStudent(int classId, int studentId, CancellationToken ct)
    {
        await _classService.RemoveStudentAsync(classId, studentId, ct);
        return Ok(ApiResponse.Ok("Student removed from class successfully."));
    }
}

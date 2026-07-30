using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Grades;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/grades")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    /// <summary>
    /// Enters a grade for a student's enrollment. Requires the CREATE_GRADE permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateGrade)]
    [ProducesResponseType(typeof(ApiResponse<GradeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeRequest request)
    {
        var grade = await _gradeService.CreateAsync(request);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<GradeResponse>.Ok(grade, "Grade entered successfully."));
    }

    /// <summary>
    /// Edits an existing grade. Requires the EDIT_GRADE permission.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = Permissions.EditGrade)]
    [ProducesResponseType(typeof(ApiResponse<GradeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] UpdateGradeRequest request)
    {
        var grade = await _gradeService.UpdateAsync(id, request);
        return Ok(ApiResponse<GradeResponse>.Ok(grade, "Grade updated successfully."));
    }

    /// <summary>
    /// Returns all grades for the student linked to the given user account.
    /// The target user must have the Student role. Requires the VIEW_SCORE permission.
    /// </summary>
    [HttpGet("student/{userId:int}")]
    [Authorize(Policy = Permissions.ViewScore)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GradeResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentScores(int userId)
    {
        var grades = await _gradeService.GetScoresByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<GradeResponse>>.Ok(grades, "Scores retrieved successfully."));
    }
}

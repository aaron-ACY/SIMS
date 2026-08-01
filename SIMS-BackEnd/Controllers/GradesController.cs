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
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeRequest request, CancellationToken ct)
    {
        var grade = await _gradeService.CreateAsync(request, ct);

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
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] UpdateGradeRequest request, CancellationToken ct)
    {
        var grade = await _gradeService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<GradeResponse>.Ok(grade, "Grade updated successfully."));
    }

    /// <summary>
    /// Returns the aggregated grade report for the student with the given student code.
    /// Requires the VIEW_SCORE permission.
    /// </summary>
    [HttpGet("student/{studentCode}")]
    [Authorize(Policy = Permissions.ViewScore)]
    [ProducesResponseType(typeof(ApiResponse<StudentGradeReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentScores(string studentCode, CancellationToken ct)
    {
        var report = await _gradeService.GetScoresByStudentCodeAsync(studentCode, ct);
        return Ok(ApiResponse<StudentGradeReportResponse>.Ok(report));
    }
}

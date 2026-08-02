using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SIMS.Application.DTOs.Grades;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Infrastructure.Settings;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/grades")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly IOptions<DataStoreSettings> _dataStore;

    public GradesController(IGradeService gradeService, IOptions<DataStoreSettings> dataStore)
    {
        _gradeService = gradeService;
        _dataStore    = dataStore;
    }

    /// <summary>
    /// Submits an assignment file for an enrollment.
    /// Creates a grade record (without a score) on first submission, or overwrites
    /// the stored file on re-submission. Requires the SUBMITTED permission (Student role).
    /// </summary>
    [HttpPost("{enrollmentId:int}/submit")]
    [Authorize(Policy = Permissions.Submitted)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<GradeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitAssignment(
        int enrollmentId,
        [FromForm] IFormFile file,
        CancellationToken ct)
    {
        // Resolve and create the files directory if it doesn't exist.
        var filesDir = _dataStore.Value.ResolvePath("files");
        Directory.CreateDirectory(filesDir);

        // Build a unique file name to avoid collisions on re-submission.
        var ext      = Path.GetExtension(file.FileName);
        var fileName = $"{enrollmentId}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(filesDir, fileName);

        await using (var stream = System.IO.File.Create(fullPath))
            await file.CopyToAsync(stream, ct);

        // Store only the relative path so the record is portable.
        var relativePath = Path.Combine("files", fileName);
        var grade = await _gradeService.SubmitAsync(enrollmentId, relativePath, ct);

        return Ok(ApiResponse<GradeResponse>.Ok(grade, "Assignment submitted successfully."));
    }

    /// <summary>
    /// Enters a grade for a student's enrollment.
    /// The student must have submitted an assignment first (SubmissionPath must be set).
    /// Requires the ENTER_GRADE permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.EnterGrade)]
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
    /// Edits an existing grade.
    /// The grade must have been formally entered first (GradedAt must be set).
    /// Requires the EDIT_GRADE permission.
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

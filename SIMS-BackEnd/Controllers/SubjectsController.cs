using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Subjects;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/subjects")]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    /// <summary>
    /// Returns the full list of subjects. Requires the VIEW_SUB permission.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewSubject)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubjectResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSubjects()
    {
        var subjects = await _subjectService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<SubjectResponse>>.Ok(subjects, "Subjects retrieved successfully."));
    }

    /// <summary>
    /// Creates a new subject. Requires the CREATE_SUB permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateSubject)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
    {
        var subject = await _subjectService.CreateAsync(request);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<SubjectResponse>.Ok(subject, "Subject created successfully."));
    }

    /// <summary>
    /// Deletes a subject by ID. Requires the DELETE_SUB permission.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.DeleteSubject)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        await _subjectService.DeleteAsync(id);
        return Ok(ApiResponse.Ok("Subject deleted successfully."));
    }
}

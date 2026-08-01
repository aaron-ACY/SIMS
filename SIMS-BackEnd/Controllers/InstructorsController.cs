using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Instructors;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/instructors")]
[Authorize]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    /// <summary>Returns all instructors with name and email resolved from the linked user.</summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewInstructors)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<InstructorResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllInstructors(CancellationToken ct)
    {
        var instructors = await _instructorService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<InstructorResponse>>.Ok(instructors));
    }

    /// <summary>Returns a single instructor by their internal ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = Permissions.ViewInstructors)]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInstructor(int id, CancellationToken ct)
    {
        var instructor = await _instructorService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<InstructorResponse>.Ok(instructor));
    }

    /// <summary>
    /// Creates a new instructor record linked to an existing user.
    /// InstructorCode must be unique.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateInstructor)]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorRequest request, CancellationToken ct)
    {
        var instructor = await _instructorService.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<InstructorResponse>.Ok(instructor, "Instructor created successfully."));
    }

    /// <summary>Updates an existing instructor record. All fields are optional.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = Permissions.EditInstructor)]
    [ProducesResponseType(typeof(ApiResponse<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateInstructor(int id, [FromBody] UpdateInstructorRequest request, CancellationToken ct)
    {
        var instructor = await _instructorService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<InstructorResponse>.Ok(instructor));
    }

    /// <summary>
    /// Deletes an instructor record. Returns 409 Conflict when the instructor
    /// is still assigned to active classes.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.DeleteInstructor)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteInstructor(int id, CancellationToken ct)
    {
        await _instructorService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null));
    }
}

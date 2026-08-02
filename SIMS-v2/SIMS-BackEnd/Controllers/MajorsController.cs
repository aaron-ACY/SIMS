using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Majors;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/majors")]
[Authorize]
public class MajorsController : ControllerBase
{
    private readonly IMajorService _majorService;

    public MajorsController(IMajorService majorService)
    {
        _majorService = majorService;
    }

    /// <summary>
    /// Returns all majors. Requires the VIEW_MAJOR permission.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewMajors)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MajorResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMajors(CancellationToken ct)
    {
        var majors = await _majorService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<MajorResponse>>.Ok(majors, "Majors retrieved successfully."));
    }

    /// <summary>
    /// Creates a new major. Requires the CREATE_MAJOR permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateMajor)]
    [ProducesResponseType(typeof(ApiResponse<MajorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateMajor([FromBody] CreateMajorRequest request, CancellationToken ct)
    {
        var major = await _majorService.CreateAsync(request, ct);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<MajorResponse>.Ok(major, "Major created successfully."));
    }

    /// <summary>
    /// Deletes a major by ID. Requires the DELETE_MAJOR permission.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.DeleteMajor)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMajor(int id, CancellationToken ct)
    {
        await _majorService.DeleteAsync(id, ct);
        return Ok(ApiResponse.Ok("Major deleted successfully."));
    }
}

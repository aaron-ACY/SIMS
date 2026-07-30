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

    /// <summary>
    /// Returns all instructors with their name and email resolved from the linked user.
    /// Requires the VIEW_INSTRUCTORS permission.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewInstructors)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<InstructorResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllInstructors()
    {
        var instructors = await _instructorService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<InstructorResponse>>.Ok(instructors));
    }
}

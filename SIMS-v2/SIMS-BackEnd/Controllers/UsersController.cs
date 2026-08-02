using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Users;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Exceptions;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Returns the profile of the currently authenticated user.
    /// Accessible by any authenticated role.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var profile = await _userService.GetMyProfileAsync(GetCurrentUserId(), ct);
        return Ok(ApiResponse<UserProfileResponse>.Ok(profile));
    }

    /// <summary>
    /// Returns all users with their role and permissions resolved.
    /// Requires the VIEW_USERS permission.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewUsers)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserProfileResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var users = await _userService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<UserProfileResponse>>.Ok(users));
    }

    /// <summary>
    /// Creates a student account and student profile in one request.
    /// Requires the CREATE_USER permission.
    /// </summary>
    [HttpPost("student")]
    [Authorize(Policy = Permissions.CreateUser)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateStudentUser([FromBody] CreateStudentUserRequest request, CancellationToken ct)
    {
        var profile = await _userService.CreateStudentUserAsync(request, ct);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<UserProfileResponse>.Ok(profile, "Student user created successfully."));
    }

    /// <summary>
    /// Creates an instructor account and instructor profile in one request.
    /// Requires the CREATE_USER permission.
    /// </summary>
    [HttpPost("instructor")]
    [Authorize(Policy = Permissions.CreateUser)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateInstructorUser([FromBody] CreateInstructorUserRequest request, CancellationToken ct)
    {
        var profile = await _userService.CreateInstructorUserAsync(request, ct);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<UserProfileResponse>.Ok(profile, "Instructor user created successfully."));
    }

    /// <summary>
    /// Creates a bare user account without a profile (Admin role or custom use).
    /// Requires the CREATE_USER permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateUser)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var profile = await _userService.CreateAsync(request, ct);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<UserProfileResponse>.Ok(profile, "User created successfully."));
    }

    /// <summary>
    /// Updates the personal information of the authenticated caller.
    /// Requires the EDIT_PROFILE permission.
    /// </summary>
    [HttpPut("me")]
    [Authorize(Policy = Permissions.EditProfile)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateMyInfo([FromBody] UpdateMyInfoRequest request, CancellationToken ct)
    {
        var profile = await _userService.UpdateMyInfoAsync(GetCurrentUserId(), request, ct);
        return Ok(ApiResponse<UserProfileResponse>.Ok(profile, "Information updated successfully."));
    }

    /// <summary>
    /// Deletes a user account by ID.
    /// Requires the DELETE_USER permission. Callers cannot delete themselves.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.DeleteUser)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
    {
        await _userService.DeleteAsync(id, GetCurrentUserId(), ct);
        return Ok(ApiResponse.Ok("User deleted successfully."));
    }

    // ------------------------------------------------------------------ //

    private int GetCurrentUserId()
    {
        if (!int.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var userId))
            throw new AppException(ErrorCode.INVALID_TOKEN, "Invalid token claims.");

        return userId;
    }
}

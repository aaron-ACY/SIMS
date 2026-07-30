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
    /// Email is loaded from the database — it is no longer stored in the JWT.
    /// Accessible by any authenticated role.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _userService.GetMyProfileAsync(GetCurrentUserId());
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
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<UserProfileResponse>>.Ok(users));
    }

    /// <summary>
    /// Creates a new user account.
    /// Requires the CREATE_USER permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreateUser)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var profile = await _userService.CreateAsync(request);

        // No GET /api/users/{id} endpoint exists to serve as the Location target,
        // so a bare 201 is returned instead of a Location header pointing nowhere.
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<UserProfileResponse>.Ok(profile, "User created successfully."));
    }

    /// <summary>
    /// Updates the personal information (email and name) of the authenticated caller.
    /// Requires the EDIT_PROFILE permission.
    /// </summary>
    [HttpPut("me")]
    [Authorize(Policy = Permissions.EditProfile)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateMyInfo([FromBody] UpdateMyInfoRequest request)
    {
        var profile = await _userService.UpdateMyInfoAsync(GetCurrentUserId(), request);

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
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteAsync(id, GetCurrentUserId());
        return Ok(ApiResponse.Ok("User deleted successfully."));
    }

    // ------------------------------------------------------------------ //

    /// <summary>
    /// Reads the caller's user ID from the sub claim.
    /// Claim mapping is disabled (see Program.cs), so the raw name is used.
    /// </summary>
    private int GetCurrentUserId()
    {
        if (!int.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var userId))
            throw new AppException(ErrorCode.INVALID_TOKEN, "Invalid token claims.");

        return userId;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Permissions;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/permissions")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Returns every permission in the system.
    /// Requires the VIEW_PERMISSIONS permission.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Permissions.ViewPermissions)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllPermissions(CancellationToken ct)
    {
        var permissions = await _permissionService.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<PermissionResponse>>.Ok(permissions));
    }

    /// <summary>
    /// Creates a new permission from a name and description.
    /// Requires the CREATE_PERMISSION permission.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CreatePermission)]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request, CancellationToken ct)
    {
        var permission = await _permissionService.CreateAsync(request, ct);

        // No GET /api/permissions/{id} endpoint exists to serve as the Location target,
        // so a bare 201 is returned instead of a Location header pointing nowhere.
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<PermissionResponse>.Ok(permission, "Permission created successfully."));
    }

    /// <summary>
    /// Updates a permission's name and/or description. Omitted fields keep their
    /// current value. Requires the EDIT_PERMISSION permission.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = Permissions.EditPermission)]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdatePermission(
        int id,
        [FromBody] UpdatePermissionRequest request,
        CancellationToken ct)
    {
        var permission = await _permissionService.UpdateAsync(id, request, ct);

        return Ok(ApiResponse<PermissionResponse>.Ok(
            permission, "Permission updated successfully."));
    }
}

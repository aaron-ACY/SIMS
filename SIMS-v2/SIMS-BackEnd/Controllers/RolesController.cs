using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Permissions;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public RolesController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Grants a permission to a role and returns the role's full permission set.
    /// Requires the GET_PERMISSION permission.
    ///
    /// Callers already holding the role keep their old permission set until they log in
    /// again — permissions are baked into the JWT at login, not read per request.
    /// </summary>
    [HttpPost("{roleId:int}/permissions")]
    [Authorize(Policy = Permissions.GetPermission)]
    [ProducesResponseType(typeof(ApiResponse<RolePermissionsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AssignPermissionToRole(
        int roleId,
        [FromBody] AssignPermissionRequest request,
        CancellationToken ct)
    {
        var result = await _permissionService.AssignToRoleAsync(roleId, request, ct);

        return Ok(ApiResponse<RolePermissionsResponse>.Ok(
            result, "Permission assigned to role successfully."));
    }
}

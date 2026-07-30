using SIMS.Application.DTOs.Permissions;

namespace SIMS.Application.Interfaces.Services;

public interface IPermissionService
{
    /// <summary>
    /// Returns every permission in the store. Requires the VIEW_PERMISSIONS permission.
    /// </summary>
    Task<IEnumerable<PermissionResponse>> GetAllAsync();

    /// <summary>
    /// Creates a permission row. Requires the CREATE_PERMISSION permission.
    /// </summary>
    Task<PermissionResponse> CreateAsync(CreatePermissionRequest request);

    /// <summary>
    /// Updates a permission's name and/or description. Requires the EDIT_PERMISSION
    /// permission. Fields left null on the request keep their current value.
    /// </summary>
    Task<PermissionResponse> UpdateAsync(int permissionId, UpdatePermissionRequest request);

    /// <summary>
    /// Grants a permission to a role. Requires the GET_PERMISSION permission.
    /// Returns the role's full permission set after the assignment.
    /// </summary>
    Task<RolePermissionsResponse> AssignToRoleAsync(int roleId, AssignPermissionRequest request);
}

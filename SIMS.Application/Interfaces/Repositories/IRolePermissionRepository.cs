using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IRolePermissionRepository
{
    Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId);

    /// <summary>True when the role already holds the permission.</summary>
    Task<bool> ExistsAsync(int roleId, int permissionId);

    Task AddAsync(RolePermission rolePermission);
}

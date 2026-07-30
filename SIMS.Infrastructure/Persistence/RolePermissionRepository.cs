using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

/// <summary>
/// Repository for the role_permissions join table.
/// Registered both as the concrete type (PermissionRepository takes it directly) and
/// as IRolePermissionRepository pointing at the same singleton — the base class holds
/// one semaphore per instance, so a second instance would mean the CSV is guarded by
/// two independent locks.
/// </summary>
public class RolePermissionRepository : CsvRepositoryBase<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("role_permissions.csv")) { }

    public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId)
    {
        var all = await ReadAllAsync();
        return all.Where(rp => rp.RoleId == roleId);
    }

    public async Task<bool> ExistsAsync(int roleId, int permissionId)
    {
        var all = await ReadAllAsync();
        return all.Any(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
    }

    public async Task AddAsync(RolePermission rolePermission)
    {
        var all = await ReadAllAsync();
        rolePermission.Id = all.Count == 0 ? 1 : all.Max(rp => rp.Id) + 1;
        all.Add(rolePermission);
        await WriteAllAsync(all);
    }
}

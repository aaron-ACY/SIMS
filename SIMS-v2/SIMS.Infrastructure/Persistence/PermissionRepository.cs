using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class PermissionRepository : CsvRepositoryBase<Permission>, IPermissionRepository
{
    private readonly RolePermissionRepository _rolePermissions;

    public PermissionRepository(
        IOptions<DataStoreSettings> settings,
        RolePermissionRepository rolePermissions)
        : base(settings.Value.ResolvePath("permissions.csv"))
    {
        _rolePermissions = rolePermissions;
    }

    public Task<IEnumerable<Permission>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId)
    {
        var mappings    = await _rolePermissions.GetByRoleIdAsync(roleId);
        var permIds     = mappings.Select(m => m.PermissionId).ToHashSet();
        var permissions = await ReadAllAsync();
        return permissions.Where(p => permIds.Contains(p.Id));
    }

    public async Task<Permission?> GetByIdAsync(int id)
    {
        var permissions = await ReadAllAsync();
        return permissions.FirstOrDefault(p => p.Id == id);
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        var permissions = await ReadAllAsync();
        return permissions.FirstOrDefault(p =>
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public Task AddAsync(Permission permission) =>
        ReadModifyWriteAsync(permissions =>
        {
            permission.Id = permissions.Count == 0 ? 1 : permissions.Max(p => p.Id) + 1;
            permissions.Add(permission);
        });

    public Task UpdateAsync(Permission permission) =>
        ReadModifyWriteAsync(permissions =>
        {
            var index = permissions.FindIndex(p => p.Id == permission.Id);
            if (index < 0) return false;
            permissions[index] = permission;
            return true;
        });
}

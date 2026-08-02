using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId);
    Task<Permission?> GetByIdAsync(int id);
    Task<Permission?> GetByNameAsync(string name);
    Task AddAsync(Permission permission);
    Task UpdateAsync(Permission permission);
}

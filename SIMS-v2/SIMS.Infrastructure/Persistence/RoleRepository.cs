using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class RoleRepository : CsvRepositoryBase<Role>, IRoleRepository
{
    public RoleRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("roles.csv")) { }

    public async Task<Role?> GetByIdAsync(int id)
    {
        var roles = await ReadAllAsync();
        return roles.FirstOrDefault(r => r.Id == id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        var roles = await ReadAllAsync();
        return roles.FirstOrDefault(r =>
            string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public Task<IEnumerable<Role>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());
}

using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class MajorRepository : CsvRepositoryBase<Major>, IMajorRepository
{
    public MajorRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("majors.csv")) { }

    public Task<IEnumerable<Major>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task<Major?> GetByIdAsync(int id)
    {
        var majors = await ReadAllAsync();
        return majors.FirstOrDefault(m => m.Id == id);
    }

    public async Task<Major?> GetByMajorCodeAsync(string majorCode)
    {
        var majors = await ReadAllAsync();
        return majors.FirstOrDefault(m =>
            string.Equals(m.MajorCode, majorCode, StringComparison.OrdinalIgnoreCase));
    }

    public Task AddAsync(Major major) =>
        ReadModifyWriteAsync(majors =>
        {
            major.Id        = majors.Count == 0 ? 1 : majors.Max(m => m.Id) + 1;
            major.CreatedAt = DateTime.UtcNow;
            major.UpdatedAt = DateTime.UtcNow;
            majors.Add(major);
        });

    public Task<bool> DeleteAsync(int id) =>
        ReadModifyWriteAsync(majors =>
        {
            var index = majors.FindIndex(m => m.Id == id);
            if (index < 0) return false;
            majors.RemoveAt(index);
            return true;
        });
}

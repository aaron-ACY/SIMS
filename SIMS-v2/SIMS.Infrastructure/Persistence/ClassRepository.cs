using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class ClassRepository : CsvRepositoryBase<Class>, IClassRepository
{
    public ClassRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("classes.csv")) { }

    public Task<IEnumerable<Class>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task<Class?> GetByIdAsync(int id)
    {
        var classes = await ReadAllAsync();
        return classes.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Class?> GetByClassCodeAsync(string classCode)
    {
        var classes = await ReadAllAsync();
        return classes.FirstOrDefault(c =>
            string.Equals(c.ClassCode, classCode, StringComparison.OrdinalIgnoreCase));
    }

    public Task AddAsync(Class schoolClass) =>
        ReadModifyWriteAsync(classes =>
        {
            schoolClass.Id        = classes.Count == 0 ? 1 : classes.Max(c => c.Id) + 1;
            schoolClass.CreatedAt = DateTime.UtcNow;
            schoolClass.UpdatedAt = DateTime.UtcNow;
            classes.Add(schoolClass);
        });

    public Task<bool> UpdateEnrollmentCountAsync(int classId, int delta) =>
        ReadModifyWriteAsync(classes =>
        {
            var schoolClass = classes.FirstOrDefault(c => c.Id == classId);
            if (schoolClass is null) return false;

            // Defence-in-depth: re-verify capacity inside the repository lock
            // so a counter can never exceed MaxEnrollment even if the service-layer
            // check is somehow bypassed.
            if (delta > 0 && schoolClass.CurrentEnrollment >= schoolClass.MaxEnrollment)
                return false;

            schoolClass.CurrentEnrollment += delta;
            schoolClass.UpdatedAt          = DateTime.UtcNow;
            return true;
        });
}

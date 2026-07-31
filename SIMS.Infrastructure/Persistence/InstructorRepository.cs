using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class InstructorRepository : CsvRepositoryBase<Instructor>, IInstructorRepository
{
    public InstructorRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("instructors.csv")) { }

    public Task<IEnumerable<Instructor>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task<Instructor?> GetByIdAsync(int id)
    {
        var instructors = await ReadAllAsync();
        return instructors.FirstOrDefault(i => i.Id == id);
    }

    public async Task<Instructor?> GetByUserIdAsync(int userId)
    {
        var instructors = await ReadAllAsync();
        return instructors.FirstOrDefault(i => i.UserId == userId);
    }
}

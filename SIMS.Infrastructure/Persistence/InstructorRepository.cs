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

    public async Task<Instructor?> GetByInstructorCodeAsync(string instructorCode)
    {
        var instructors = await ReadAllAsync();
        return instructors.FirstOrDefault(
            i => string.Equals(i.InstructorCode, instructorCode, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Instructor?> GetByEmailAsync(string email)
    {
        var instructors = await ReadAllAsync();
        return instructors.FirstOrDefault(i =>
            string.Equals(i.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public Task AddAsync(Instructor instructor) =>
        ReadModifyWriteAsync(instructors =>
        {
            instructor.Id        = instructors.Count == 0 ? 1 : instructors.Max(i => i.Id) + 1;
            instructor.CreatedAt = DateTime.UtcNow;
            instructor.UpdatedAt = DateTime.UtcNow;
            instructors.Add(instructor);
        });

    public Task<bool> UpdateAsync(Instructor instructor) =>
        ReadModifyWriteAsync(instructors =>
        {
            var index = instructors.FindIndex(i => i.Id == instructor.Id);
            if (index < 0) return false;
            instructors[index] = instructor;
            return true;
        });

    public Task<bool> DeleteAsync(int id) =>
        ReadModifyWriteAsync(instructors =>
        {
            var index = instructors.FindIndex(i => i.Id == id);
            if (index < 0) return false;
            instructors.RemoveAt(index);
            return true;
        });
}

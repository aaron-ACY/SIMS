using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class SubjectRepository : CsvRepositoryBase<Subject>, ISubjectRepository
{
    public SubjectRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("subjects.csv")) { }

    public Task<IEnumerable<Subject>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task<Subject?> GetByIdAsync(int id)
    {
        var subjects = await ReadAllAsync();
        return subjects.FirstOrDefault(s => s.Id == id);
    }

    public async Task<Subject?> GetBySubjectCodeAsync(string subjectCode)
    {
        var subjects = await ReadAllAsync();
        return subjects.FirstOrDefault(s =>
            string.Equals(s.SubjectCode, subjectCode, StringComparison.OrdinalIgnoreCase));
    }

    public async Task AddAsync(Subject subject)
    {
        var subjects = await ReadAllAsync();
        subject.Id        = subjects.Count == 0 ? 1 : subjects.Max(s => s.Id) + 1;
        subject.CreatedAt = DateTime.UtcNow;
        subject.UpdatedAt = DateTime.UtcNow;
        subjects.Add(subject);
        await WriteAllAsync(subjects);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subjects = await ReadAllAsync();
        var index = subjects.FindIndex(s => s.Id == id);
        if (index < 0) return false;
        subjects.RemoveAt(index);
        await WriteAllAsync(subjects);
        return true;
    }
}

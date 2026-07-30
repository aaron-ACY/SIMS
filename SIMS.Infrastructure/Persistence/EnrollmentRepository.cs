using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class EnrollmentRepository : CsvRepositoryBase<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("enrollments.csv")) { }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        var enrollments = await ReadAllAsync();
        return enrollments.FirstOrDefault(e => e.Id == id && e.IsActive);
    }

    public async Task<IEnumerable<Enrollment>> GetByClassIdAsync(int classId)
    {
        var enrollments = await ReadAllAsync();
        return enrollments.Where(e => e.ClassId == classId && e.IsActive);
    }

    public async Task<Enrollment?> GetAsync(int classId, int studentId)
    {
        var enrollments = await ReadAllAsync();
        return enrollments.FirstOrDefault(e =>
            e.ClassId == classId && e.StudentId == studentId && e.IsActive);
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        var enrollments = await ReadAllAsync();
        enrollment.Id = enrollments.Count == 0 ? 1 : enrollments.Max(e => e.Id) + 1;
        enrollments.Add(enrollment);
        await WriteAllAsync(enrollments);
    }

    public async Task<bool> DeleteAsync(int classId, int studentId)
    {
        var enrollments = await ReadAllAsync();
        var index = enrollments.FindIndex(
            e => e.ClassId == classId && e.StudentId == studentId && e.IsActive);

        if (index < 0) return false;

        // Soft-delete: mark inactive rather than removing the record.
        enrollments[index].IsActive = false;
        await WriteAllAsync(enrollments);
        return true;
    }
}

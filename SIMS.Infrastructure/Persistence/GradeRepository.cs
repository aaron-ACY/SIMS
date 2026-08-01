using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class GradeRepository : CsvRepositoryBase<Grade>, IGradeRepository
{
    public GradeRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("grades.csv")) { }

    public async Task<Grade?> GetByIdAsync(int id)
    {
        var grades = await ReadAllAsync();
        return grades.FirstOrDefault(g => g.Id == id);
    }

    public async Task<Grade?> GetByEnrollmentIdAsync(int enrollmentId)
    {
        var grades = await ReadAllAsync();
        return grades.FirstOrDefault(g => g.EnrollmentId == enrollmentId);
    }

    public async Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId)
    {
        var grades = await ReadAllAsync();
        return grades.Where(g => g.StudentId == studentId);
    }

    public Task AddAsync(Grade grade) =>
        ReadModifyWriteAsync(grades =>
        {
            grade.Id = grades.Count == 0 ? 1 : grades.Max(g => g.Id) + 1;
            grades.Add(grade);
        });

    public Task<bool> UpdateAsync(Grade grade) =>
        ReadModifyWriteAsync(grades =>
        {
            var index = grades.FindIndex(g => g.Id == grade.Id);
            if (index < 0) return false;
            grades[index] = grade;
            return true;
        });
}

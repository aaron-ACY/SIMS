using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class CourseRepository : CsvRepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("courses.csv")) { }

    public Task<IEnumerable<Course>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task<Course?> GetByIdAsync(int id)
    {
        var courses = await ReadAllAsync();
        return courses.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Course?> GetByCourseCodeAsync(string courseCode)
    {
        var courses = await ReadAllAsync();
        return courses.FirstOrDefault(c =>
            string.Equals(c.CourseCode, courseCode, StringComparison.OrdinalIgnoreCase));
    }

    public Task AddAsync(Course course) =>
        ReadModifyWriteAsync(courses =>
        {
            course.Id        = courses.Count == 0 ? 1 : courses.Max(c => c.Id) + 1;
            course.CreatedAt = DateTime.UtcNow;
            course.UpdatedAt = DateTime.UtcNow;
            courses.Add(course);
        });

    public Task<bool> DeleteAsync(int id) =>
        ReadModifyWriteAsync(courses =>
        {
            var index = courses.FindIndex(c => c.Id == id);
            if (index < 0) return false;
            courses.RemoveAt(index);
            return true;
        });
}

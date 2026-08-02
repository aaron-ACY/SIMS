using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<Course?> GetByCourseCodeAsync(string courseCode);
    Task AddAsync(Course course);

    /// <summary>Removes the course with the given ID. Returns false when no such course exists.</summary>
    Task<bool> DeleteAsync(int id);
}

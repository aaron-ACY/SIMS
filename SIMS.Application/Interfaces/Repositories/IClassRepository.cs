using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IClassRepository
{
    Task<IEnumerable<Class>> GetAllAsync();
    Task<Class?> GetByIdAsync(int id);
    Task<Class?> GetByClassCodeAsync(string classCode);
    Task AddAsync(Class schoolClass);

    /// <summary>
    /// Increments or decrements CurrentEnrollment by <paramref name="delta"/> (+1 or -1)
    /// and persists the change. Returns false when the class does not exist.
    /// </summary>
    Task<bool> UpdateEnrollmentCountAsync(int classId, int delta);
}

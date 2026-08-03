using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(int id);
    Task<Grade?> GetByEnrollmentIdAsync(int enrollmentId);

    /// <summary>Returns all grade records that belong to the given student.</summary>
    Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId);

    /// <summary>Returns all grade records for every enrollment in the given class.</summary>
    Task<IEnumerable<Grade>> GetByClassIdAsync(int classId);

    Task AddAsync(Grade grade);

    /// <summary>Persists changes to an existing grade. Returns false when not found.</summary>
    Task<bool> UpdateAsync(Grade grade);
}

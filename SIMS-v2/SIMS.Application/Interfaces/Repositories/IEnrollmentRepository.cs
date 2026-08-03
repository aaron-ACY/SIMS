using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id);
    Task<IEnumerable<Enrollment>> GetByClassIdAsync(int classId);

    /// <summary>Returns all active enrollments for the given student.</summary>
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId);

    Task<Enrollment?> GetAsync(int classId, int studentId);
    Task AddAsync(Enrollment enrollment);

    /// <summary>
    /// Removes the enrollment for the given class/student pair.
    /// Returns false when no active enrollment is found.
    /// </summary>
    Task<bool> DeleteAsync(int classId, int studentId);

    /// <summary>
    /// Returns <c>true</c> when the student has at least one active enrollment.
    /// Used to guard against deleting a student who is still enrolled in classes.
    /// </summary>
    Task<bool> ExistsActiveForStudentAsync(int studentId);
}

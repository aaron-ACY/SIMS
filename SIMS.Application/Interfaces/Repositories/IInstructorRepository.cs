using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IInstructorRepository
{
    Task<IEnumerable<Instructor>> GetAllAsync();
    Task<Instructor?> GetByIdAsync(int id);

    /// <summary>Returns the instructor record linked to the given user account, or null when not found.</summary>
    Task<Instructor?> GetByUserIdAsync(int userId);

    /// <summary>Returns the instructor record with the given code (case-insensitive), or null when not found.</summary>
    Task<Instructor?> GetByInstructorCodeAsync(string instructorCode);

    Task AddAsync(Instructor instructor);

    /// <summary>Persists changes to an existing instructor. Returns false when not found.</summary>
    Task<bool> UpdateAsync(Instructor instructor);

    /// <summary>
    /// Deletes the instructor record with the given ID.
    /// Returns false when no record is found.
    /// </summary>
    Task<bool> DeleteAsync(int id);
}

using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IInstructorRepository
{
    Task<IEnumerable<Instructor>> GetAllAsync();
    Task<Instructor?> GetByIdAsync(int id);

    /// <summary>Returns the instructor record linked to the given user account, or null when not found.</summary>
    Task<Instructor?> GetByUserIdAsync(int userId);
}

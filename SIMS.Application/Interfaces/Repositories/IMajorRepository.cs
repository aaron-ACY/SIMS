using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IMajorRepository
{
    Task<IEnumerable<Major>> GetAllAsync();
    Task<Major?> GetByIdAsync(int id);
    Task<Major?> GetByMajorCodeAsync(string majorCode);
    Task AddAsync(Major major);

    /// <summary>Removes the major with the given ID. Returns false when no such major exists.</summary>
    Task<bool> DeleteAsync(int id);
}

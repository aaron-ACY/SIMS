using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IInstructorRepository
{
    Task<IEnumerable<Instructor>> GetAllAsync();
    Task<Instructor?> GetByIdAsync(int id);
}

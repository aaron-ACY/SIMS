using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface ISubjectRepository
{
    Task<IEnumerable<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(int id);
    Task<Subject?> GetBySubjectCodeAsync(string subjectCode);
    Task AddAsync(Subject subject);

    /// <summary>Removes the subject with the given ID. Returns false when no such subject exists.</summary>
    Task<bool> DeleteAsync(int id);
}

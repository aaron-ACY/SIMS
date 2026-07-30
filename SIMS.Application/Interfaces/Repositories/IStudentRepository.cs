using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);

    /// <summary>Returns the student record linked to the given user account, or null when not found.</summary>
    Task<Student?> GetByUserIdAsync(int userId);

    /// <summary>Returns the student record with the given student code, or null when not found.</summary>
    Task<Student?> GetByStudentCodeAsync(string studentCode);

    Task AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task<bool> DeleteAsync(int id);
}


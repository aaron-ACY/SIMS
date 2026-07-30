using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);

    /// <summary>Removes the user with the given ID. Returns false when no such user exists.</summary>
    Task<bool> DeleteAsync(int id);
}

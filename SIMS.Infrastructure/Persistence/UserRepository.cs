using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class UserRepository : CsvRepositoryBase<User>, IUserRepository
{
    public UserRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("users.csv")) { }

    public async Task<User?> GetByIdAsync(int id)
    {
        var users = await ReadAllAsync();
        return users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var users = await ReadAllAsync();
        return users.FirstOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var users = await ReadAllAsync();
        return users.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public Task<IEnumerable<User>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task AddAsync(User user)
    {
        var users = await ReadAllAsync();
        user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        users.Add(user);
        await WriteAllAsync(users);
    }

    public async Task UpdateAsync(User user)
    {
        var users = await ReadAllAsync();
        var index = users.FindIndex(u => u.Id == user.Id);
        if (index < 0) return;
        user.UpdatedAt = DateTime.UtcNow;
        users[index] = user;
        await WriteAllAsync(users);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var users = await ReadAllAsync();
        var index = users.FindIndex(u => u.Id == id);
        if (index < 0) return false;
        users.RemoveAt(index);
        await WriteAllAsync(users);
        return true;
    }
}

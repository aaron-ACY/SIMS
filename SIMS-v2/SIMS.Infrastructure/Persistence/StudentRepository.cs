using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class StudentRepository : CsvRepositoryBase<Student>, IStudentRepository
{
    public StudentRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("students.csv")) { }

    public Task<IEnumerable<Student>> GetAllAsync() =>
        ReadAllAsync().ContinueWith(t => t.Result.AsEnumerable());

    public async Task<Student?> GetByIdAsync(int id)
    {
        var students = await ReadAllAsync();
        return students.FirstOrDefault(s => s.Id == id);
    }

    public async Task<Student?> GetByUserIdAsync(int userId)
    {
        var students = await ReadAllAsync();
        return students.FirstOrDefault(s => s.UserId == userId);
    }

    public async Task<Student?> GetByStudentCodeAsync(string studentCode)
    {
        var students = await ReadAllAsync();
        return students.FirstOrDefault(s =>
            string.Equals(s.StudentCode, studentCode, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        var students = await ReadAllAsync();
        return students.FirstOrDefault(s =>
            string.Equals(s.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public Task AddAsync(Student student) =>
        ReadModifyWriteAsync(students =>
        {
            student.Id        = students.Count == 0 ? 1 : students.Max(s => s.Id) + 1;
            student.CreatedAt = DateTime.UtcNow;
            student.UpdatedAt = DateTime.UtcNow;
            students.Add(student);
        });

    public Task UpdateAsync(Student student) =>
        ReadModifyWriteAsync(students =>
        {
            var index = students.FindIndex(s => s.Id == student.Id);
            if (index < 0) return false;
            student.UpdatedAt  = DateTime.UtcNow;
            students[index]    = student;
            return true;
        });

    public Task<bool> DeleteAsync(int id) =>
        ReadModifyWriteAsync(students =>
        {
            var index = students.FindIndex(s => s.Id == id);
            if (index < 0) return false;
            students.RemoveAt(index);
            return true;
        });
}


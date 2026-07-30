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

    public async Task AddAsync(Student student)
    {
        var students = await ReadAllAsync();
        student.Id        = students.Count == 0 ? 1 : students.Max(s => s.Id) + 1;
        student.CreatedAt = DateTime.UtcNow;
        student.UpdatedAt = DateTime.UtcNow;
        students.Add(student);
        await WriteAllAsync(students);
    }

    public async Task UpdateAsync(Student student)
    {
        var students = await ReadAllAsync();
        var index = students.FindIndex(s => s.Id == student.Id);
        if (index < 0) return;
        student.UpdatedAt = DateTime.UtcNow;
        students[index] = student;
        await WriteAllAsync(students);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var students = await ReadAllAsync();
        var index = students.FindIndex(s => s.Id == id);
        if (index < 0) return false;
        students.RemoveAt(index);
        await WriteAllAsync(students);
        return true;
    }
}


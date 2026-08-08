using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;

namespace SIMS.Tests.Unit.Support;

/// <summary>
/// In-memory repository context dùng cho unit test: thay thế StudentRepository (CSV)
/// bằng một store nằm hoàn toàn trong bộ nhớ, không chạm tới đĩa.
///
/// Hai điểm khác biệt có chủ đích so với repository thật:
///   1. StudentCode được coi là khoá định danh duy nhất (unique constraint). Khi
///      <see cref="AddAsync"/> nhận một record có StudentCode đã tồn tại, store ném
///      <see cref="InvalidOperationException"/> — bản ghi trùng bị chặn ngay tại
///      tầng dữ liệu thay vì được ghi thêm.
///   2. Dữ liệu được clone khi đọc/ghi, nên caller không thể sửa trực tiếp state
///      bên trong store qua reference. Nhờ đó các assertion kiểu "record đã bị xoá
///      hẳn khỏi repository" mới có ý nghĩa.
/// </summary>
public sealed class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new();
    private int _nextId = 1;

    /// <summary>Số record hiện có trong store.</summary>
    public int Count => _students.Count;

    // ── Read ───────────────────────────────────────────────────────────── //

    public Task<IEnumerable<Student>> GetAllAsync() =>
        Task.FromResult(_students.Select(Clone).AsEnumerable());

    public Task<Student?> GetByIdAsync(int id) =>
        Task.FromResult(FindClone(s => s.Id == id));

    public Task<Student?> GetByUserIdAsync(int userId) =>
        Task.FromResult(FindClone(s => s.UserId == userId));

    public Task<Student?> GetByStudentCodeAsync(string studentCode) =>
        Task.FromResult(FindClone(s => SameCode(s.StudentCode, studentCode)));

    public Task<Student?> GetByEmailAsync(string email) =>
        Task.FromResult(FindClone(s =>
            string.Equals(s.Email, email, StringComparison.OrdinalIgnoreCase)));

    // ── Write ──────────────────────────────────────────────────────────── //

    /// <summary>
    /// Thêm record mới. Ném <see cref="InvalidOperationException"/> nếu StudentCode
    /// đã tồn tại (so sánh không phân biệt hoa/thường).
    /// </summary>
    public Task AddAsync(Student student)
    {
        if (_students.Any(s => SameCode(s.StudentCode, student.StudentCode)))
            throw new InvalidOperationException(
                $"A student with Id '{student.StudentCode}' already exists in the repository.");

        student.Id        = _nextId++;
        student.CreatedAt = DateTime.UtcNow;
        student.UpdatedAt = DateTime.UtcNow;

        _students.Add(Clone(student));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Student student)
    {
        var index = _students.FindIndex(s => s.Id == student.Id);
        if (index < 0) return Task.CompletedTask;

        student.UpdatedAt = DateTime.UtcNow;
        _students[index]  = Clone(student);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(int id)
    {
        var index = _students.FindIndex(s => s.Id == id);
        if (index < 0) return Task.FromResult(false);

        _students.RemoveAt(index);
        return Task.FromResult(true);
    }

    // ── Helpers ────────────────────────────────────────────────────────── //

    private Student? FindClone(Func<Student, bool> predicate)
    {
        var found = _students.FirstOrDefault(predicate);
        return found is null ? null : Clone(found);
    }

    private static bool SameCode(string left, string right) =>
        string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

    private static Student Clone(Student s) => new()
    {
        Id             = s.Id,
        UserId         = s.UserId,
        StudentCode    = s.StudentCode,
        FirstName      = s.FirstName,
        LastName       = s.LastName,
        Email          = s.Email,
        DateOfBirth    = s.DateOfBirth,
        Gender         = s.Gender,
        Phone          = s.Phone,
        Address        = s.Address,
        Major          = s.Major,
        EnrollmentYear = s.EnrollmentYear,
        Status         = s.Status,
        IsActive       = s.IsActive,
        CreatedAt      = s.CreatedAt,
        UpdatedAt      = s.UpdatedAt
    };
}

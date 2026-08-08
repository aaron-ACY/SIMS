using FluentAssertions;
using Moq;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Services;
using SIMS.Domain.Entities;
using SIMS.Tests.Unit.Support;
using Xunit;

namespace SIMS.Tests.Unit.Students;

[Trait("Category", "Unit")]
public class StudentRepositoryUnitTests
{
    private const string TargetStudentId = "BD00889";

    private readonly InMemoryStudentRepository _repository = new();
    private readonly Mock<IUserRepository>     _userRepo   = new();

    private StudentService BuildService() => new(_repository, _userRepo.Object);

    private static Student NewStudent(string studentCode) => new()
    {
        UserId         = null,
        StudentCode    = studentCode,
        FirstName      = "Tran",
        LastName       = "Ngoc",
        Email          = $"{studentCode.ToLowerInvariant()}@sims.edu",
        DateOfBirth    = new DateTime(2005, 3, 15, 0, 0, 0, DateTimeKind.Utc),
        Gender         = "Female",
        Phone          = "0912345678",
        Address        = "123 Le Loi - Da Nang",
        Major          = "Information Technology",
        EnrollmentYear = 2023,
        Status         = "Active",
        IsActive       = true
    };

    [Fact]
    public async Task STU_ADD_004_DuplicateStudentId()
    {
        // ── Arrange ────────────────────────────────────────────────────── //
        var first = NewStudent(TargetStudentId);
        await _repository.AddAsync(first);

        var duplicate = NewStudent(TargetStudentId);
        duplicate.Email = "another.person@sims.edu";   

        // ── Act ────────────────────────────────────────────────────────── //
        var act = async () => await _repository.AddAsync(duplicate);

        // ── Assert ─────────────────────────────────────────────────────── //
        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain(TargetStudentId);

        _repository.Count.Should().Be(1);

        var all = await _repository.GetAllAsync();
        all.Should().ContainSingle(s => s.StudentCode == TargetStudentId)
           .Which.Email.Should().Be("bd00889@sims.edu",
               "The original record must remain intact and not be overwritten by a duplicate record.");

        duplicate.Id.Should().Be(0);
    }

    [Fact]
    public async Task STU_SEARCH_005_NonExistingStudentId()
    {
        // ── Arrange ────────────────────────────────────────────────────── //
        var sut = BuildService();
        sut.Should().NotBeNull();

        // ── Act ────────────────────────────────────────────────────────── //
        Student? result = null;
        var act = async () => result = await _repository.GetByStudentCodeAsync("nonexistent");

        // ── Assert ─────────────────────────────────────────────────────── //
        await act.Should().NotThrowAsync();

        result.Should().BeNull();

        _repository.Count.Should().Be(0);
    }

    [Fact]
    public async Task STU_DELETE_006_ExistingStudent()
    {
        // ── Arrange ────────────────────────────────────────────────────── //
        await _repository.AddAsync(NewStudent(TargetStudentId));

        var seeded = await _repository.GetByStudentCodeAsync(TargetStudentId);
        seeded.Should().NotBeNull("BD00889 phải tồn tại trước khi xoá");

        // ── Act ────────────────────────────────────────────────────────── //
        var deleted = await _repository.DeleteAsync(seeded!.Id);

        // ── Assert ─────────────────────────────────────────────────────── //
        deleted.Should().BeTrue();

        (await _repository.GetByStudentCodeAsync(TargetStudentId)).Should().BeNull();

        (await _repository.GetByIdAsync(seeded.Id)).Should().BeNull();
        _repository.Count.Should().Be(0);
        (await _repository.GetAllAsync()).Should().BeEmpty();

        (await _repository.DeleteAsync(seeded.Id)).Should().BeFalse();
    }
}

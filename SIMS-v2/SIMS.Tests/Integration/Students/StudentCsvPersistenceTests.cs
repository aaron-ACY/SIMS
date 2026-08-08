using FluentAssertions;
using Microsoft.Extensions.Options;
using SIMS.Application.DTOs.Students;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Infrastructure.Persistence;
using SIMS.Infrastructure.Settings;
using SIMS.Shared.Exceptions;
using Xunit;

namespace SIMS.Tests.Integration.Students;

[Trait("Category", "Integration")]
public sealed class StudentCsvPersistenceTests : IDisposable
{
    private const string TargetStudentCode = "BD00889";

    private readonly CsvDataStoreFixture _fixture = new();

    public void Dispose() => _fixture.Dispose();

    private StudentRepository NewRepositoryReadingFromDisk() =>
        new(Options.Create(new DataStoreSettings { BasePath = _fixture.DataDirectory }));

    [Fact]
    public async Task STU_CSV_007_AddStudentSuccessfully()
    {
        // ── Arrange ────────────────────────────────────────────────────── //
        _fixture.SeedUsers(
            "1,tranngoczit,user@example.com,hash,salt,Ngoc,Zit,3,True," +
            "2026-01-01T00:00:00Z,2026-01-01T00:00:00Z");

        _fixture.SeedStudents();

        var service = _fixture.GetService<IStudentService>();

        var request = new CreateStudentRequest
        {
            UserId         = 1,
            StudentCode    = TargetStudentCode,
            DateOfBirth    = new DateTime(2005, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            Gender         = "Female",
            Phone          = "0912345678",
            Address        = "123 Le Loi - Da Nang",
            Major          = "Information Technology",
            EnrollmentYear = 2023,
            Status         = "Active"
        };

        // ── Act ────────────────────────────────────────────────────────── //
        var act    = async () => await service.CreateAsync(request);
        var result = await act.Should().NotThrowAsync();   

        // ── Assert ─────────────────────────────────────────────────────── //
        result.Subject.StudentCode.Should().Be(TargetStudentCode);
        result.Subject.FullName.Should().Be("Ngoc Zit");

        _fixture.FileExists("students.csv").Should().BeTrue();

        var rawCsv = _fixture.ReadRawCsv("students.csv");
        rawCsv.Should().Contain(TargetStudentCode);

        var persisted = await NewRepositoryReadingFromDisk()
                              .GetByStudentCodeAsync(TargetStudentCode);

        persisted.Should().NotBeNull();
        persisted!.StudentCode.Should().Be(TargetStudentCode);
        persisted.UserId.Should().Be(1);
        persisted.Email.Should().Be("user@example.com");
        persisted.Major.Should().Be("Information Technology");
        persisted.EnrollmentYear.Should().Be(2023);
        persisted.Status.Should().Be("Active");
        persisted.IsActive.Should().BeTrue();
        persisted.DateOfBirth.Should().Be(new DateTime(2005, 3, 15, 0, 0, 0, DateTimeKind.Utc));

        persisted.Id.Should().Be(1);
    }

    [Fact]
    public async Task STU_CSV_008_DeleteStudentSuccessfully()
    {
        // ── Arrange ────────────────────────────────────────────────────── //

        _fixture.SeedStudents(
            "889,,BD00889,Tran,Ngoc,bd00889@sims.edu,2005-03-15T00:00:00Z,Female," +
            "0912345678,123 Le Loi - Da Nang,Information Technology,2023,Active,True," +
            "2026-01-01T00:00:00Z,2026-01-01T00:00:00Z",

            "890,,BD00890,Le,Minh,bd00890@sims.edu,2004-05-20T00:00:00Z,Male," +
            "0913456789,456 Tran Phu - Ha Noi,Computer Science,2022,Active,True," +
            "2026-01-01T00:00:00Z,2026-01-01T00:00:00Z");

        _fixture.SeedUsers();  

        var service    = _fixture.GetService<IStudentService>();
        var repository = _fixture.GetService<IStudentRepository>();

        var seeded = await repository.GetByStudentCodeAsync(TargetStudentCode);
        seeded.Should().NotBeNull("Record BD00889 must exist before deletion.");
        var studentId = seeded!.Id;

        // ── Act ────────────────────────────────────────────────────────── //
        var act = async () => await service.DeleteAsync(studentId);
        await act.Should().NotThrowAsync();

        // ── Assert ─────────────────────────────────────────────────────── //
        var rawCsv = _fixture.ReadRawCsv("students.csv");
        rawCsv.Should().NotContain(TargetStudentCode);

        rawCsv.Should().Contain("BD00890");

        var afterDeleteFromDisk = await NewRepositoryReadingFromDisk()
                                        .GetByStudentCodeAsync(TargetStudentCode);
        afterDeleteFromDisk.Should().BeNull();

        (await repository.GetByIdAsync(studentId)).Should().BeNull();

        var remaining = await repository.GetAllAsync();
        remaining.Should().HaveCount(1);
        remaining.Single().StudentCode.Should().Be("BD00890");

        var getAct = async () => await service.GetByIdAsync(studentId);
        await getAct.Should().ThrowAsync<AppException>()
                    .Where(e => e.ErrorCode == ErrorCode.STUDENT_NOT_EXISTED);

        var deleteAgain = async () => await service.DeleteAsync(studentId);
        await deleteAgain.Should().ThrowAsync<AppException>()
                         .Where(e => e.ErrorCode == ErrorCode.STUDENT_NOT_EXISTED);
    }
}

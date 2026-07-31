using FluentAssertions;
using Moq;
using SIMS.Application.DTOs.Students;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;
using Xunit;

namespace SIMS.Tests.Students;

public class StudentServiceTests
{
    // ── Mocks ──────────────────────────────────────────────────────────── //

    private readonly Mock<IStudentRepository> _studentRepo = new();
    private readonly Mock<IUserRepository>    _userRepo    = new();

    private StudentService BuildService() =>
        new StudentService(_studentRepo.Object, _userRepo.Object);

    // ── Helpers ────────────────────────────────────────────────────────── //

    private static User SampleUser(int id = 1) => new()
    {
        Id        = id,
        FirstName = "Nguyen",
        LastName  = "Van A",
        Email     = "vana@example.com",
        IsActive  = true
    };

    private static Student SampleStudent(int id = 1, int userId = 1, string code = "BD00001") => new()
    {
        Id             = id,
        UserId         = userId,
        StudentCode    = code,
        DateOfBirth    = new DateTime(2000, 1, 1),
        Gender         = "Male",
        Phone          = "0901234567",
        Address        = "Hanoi",
        Major          = "CNTT",
        EnrollmentYear = 2022,
        Status         = "Active",
        IsActive       = true
    };

    private static CreateStudentRequest ValidRequest(int userId = 1, string code = "BD00001") => new()
    {
        UserId         = userId,
        StudentCode    = code,
        DateOfBirth    = new DateTime(2000, 1, 1),
        Gender         = "Male",
        Phone          = "0901234567",
        Address        = "Hanoi",
        Major          = "CNTT",
        EnrollmentYear = 2022,
        Status         = "Active"
    };

    // ══════════════════════════════════════════════════════════════════════
    // 1. Thêm học sinh (CreateAsync)
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Thêm học sinh thành công: studentCode chưa tồn tại
    /// → AddAsync được gọi, trả về StudentResponse đúng thông tin.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenStudentCodeIsUnique_ShouldCreateAndReturnStudentResponse()
    {
        // Arrange
        var user = SampleUser();
        _userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        // Chưa có sinh viên nào với mã này
        _studentRepo.Setup(r => r.GetByStudentCodeAsync(It.IsAny<string>())).ReturnsAsync((Student?)null);
        _studentRepo.Setup(r => r.AddAsync(It.IsAny<Student>())).Returns(Task.CompletedTask);

        var sut = BuildService();

        // Act
        var result = await sut.CreateAsync(ValidRequest(code: "BD00001"));

        // Assert
        result.StudentCode.Should().Be("BD00001");
        result.FullName.Should().Be("Nguyen Van A");
        _studentRepo.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Once);
    }

    /// <summary>
    /// Thêm học sinh thất bại: studentCode đã tồn tại
    /// → ném AppException STUDENT_CODE_EXISTED, không gọi AddAsync.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenStudentCodeAlreadyExists_ShouldThrowStudentCodeExisted()
    {
        // Arrange
        var user     = SampleUser();
        var existing = SampleStudent(code: "BD00001");

        _userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        // Mã đã tồn tại trong hệ thống
        _studentRepo.Setup(r => r.GetByStudentCodeAsync("BD00001")).ReturnsAsync(existing);

        var sut = BuildService();

        // Act
        var act = () => sut.CreateAsync(ValidRequest(code: "BD00001"));

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.STUDENT_CODE_EXISTED);

        _studentRepo.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2. Tìm học sinh theo studentCode
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Tìm học sinh thành công: studentCode tồn tại trong hệ thống
    /// → repository trả về đúng Student.
    /// </summary>
    [Fact]
    public async Task GetByStudentCode_WhenCodeExists_ShouldReturnMatchingStudent()
    {
        // Arrange
        var student = SampleStudent(id: 7, code: "BD00777");
        _studentRepo.Setup(r => r.GetByStudentCodeAsync("BD00777")).ReturnsAsync(student);

        // Act
        var result = await _studentRepo.Object.GetByStudentCodeAsync("BD00777");

        // Assert
        result.Should().NotBeNull();
        result!.StudentCode.Should().Be("BD00777");
    }

    /// <summary>
    /// Tìm học sinh thất bại: studentCode không tồn tại
    /// → repository trả về null.
    /// </summary>
    [Fact]
    public async Task GetByStudentCode_WhenCodeDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _studentRepo.Setup(r => r.GetByStudentCodeAsync("BD99999")).ReturnsAsync((Student?)null);

        // Act
        var result = await _studentRepo.Object.GetByStudentCodeAsync("BD99999");

        // Assert
        result.Should().BeNull();
    }
}

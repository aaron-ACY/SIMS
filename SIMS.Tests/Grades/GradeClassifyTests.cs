using FluentAssertions;
using Moq;
using SIMS.Application.DTOs.Grades;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;
using Xunit;

namespace SIMS.Tests.Grades;

public class GradeServiceTests
{
    // ── Mocks ──────────────────────────────────────────────────────────── //

    private readonly Mock<IGradeRepository>      _gradeRepo      = new();
    private readonly Mock<IEnrollmentRepository> _enrollmentRepo = new();
    private readonly Mock<IStudentRepository>    _studentRepo    = new();
    private readonly Mock<IClassRepository>      _classRepo      = new();
    private readonly Mock<ISubjectRepository>    _subjectRepo    = new();
    private readonly Mock<IUserRepository>       _userRepo       = new();

    private GradeService BuildService() => new(
        _gradeRepo.Object,
        _enrollmentRepo.Object,
        _studentRepo.Object,
        _classRepo.Object,
        _subjectRepo.Object,
        _userRepo.Object);

    // ── Helpers ────────────────────────────────────────────────────────── //

    /// Thiết lập mock cho BuildResponseAsync (student, class, user).
    private void SetupBuildResponse(int studentId, int classId, int userId)
    {
        _studentRepo.Setup(r => r.GetByIdAsync(studentId))
                    .ReturnsAsync(new Student { Id = studentId, UserId = userId });
        _classRepo.Setup(r => r.GetByIdAsync(classId))
                  .ReturnsAsync(new Class { Id = classId, ClassCode = "IT101-01" });
        _userRepo.Setup(r => r.GetByIdAsync(userId))
                 .ReturnsAsync(new User { Id = userId, FirstName = "Nguyen", LastName = "Van A" });
    }

    // ══════════════════════════════════════════════════════════════════════
    // 1. Nhập điểm (CreateAsync)
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Nhập điểm hợp lệ: enrollment tồn tại, chưa có điểm
    /// → tạo grade thành công và trả về GradeResponse.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenEnrollmentExistsAndNoPriorGrade_ShouldCreateAndReturnGradeResponse()
    {
        // Arrange
        var enrollment = new Enrollment { Id = 1, StudentId = 10, ClassId = 20 };
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(enrollment);
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync((Grade?)null);
        _gradeRepo.Setup(r => r.AddAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
        SetupBuildResponse(studentId: 10, classId: 20, userId: 5);

        var sut = BuildService();

        // Act
        var result = await sut.CreateAsync(new CreateGradeRequest { EnrollmentId = 1, Score = 7.5 });

        // Assert
        result.Should().NotBeNull();
        result.Score.Should().Be(7.5);
        result.Classification.Should().Be("Pass");
        _gradeRepo.Verify(r => r.AddAsync(It.IsAny<Grade>()), Times.Once);
    }

    /// <summary>
    /// Nhập điểm không hợp lệ: enrollment không tồn tại
    /// → ném AppException ENROLLMENT_NOT_EXISTED.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenEnrollmentDoesNotExist_ShouldThrowEnrollmentNotExisted()
    {
        // Arrange
        _enrollmentRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Enrollment?)null);

        var sut = BuildService();

        // Act
        var act = () => sut.CreateAsync(new CreateGradeRequest { EnrollmentId = 99, Score = 7.5 });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.ENROLLMENT_NOT_EXISTED);

        _gradeRepo.Verify(r => r.AddAsync(It.IsAny<Grade>()), Times.Never);
    }

    /// <summary>
    /// Nhập điểm không hợp lệ: enrollment đã có điểm rồi
    /// → ném AppException GRADE_ALREADY_EXISTS.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenGradeAlreadyExists_ShouldThrowGradeAlreadyExists()
    {
        // Arrange
        var enrollment   = new Enrollment { Id = 1, StudentId = 10, ClassId = 20 };
        var existingGrade = new Grade { Id = 5, EnrollmentId = 1 };

        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(enrollment);
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync(existingGrade);

        var sut = BuildService();

        // Act
        var act = () => sut.CreateAsync(new CreateGradeRequest { EnrollmentId = 1, Score = 8.0 });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.GRADE_ALREADY_EXISTS);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2. Chỉnh sửa điểm (UpdateAsync)
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Chỉnh sửa điểm hợp lệ: grade tồn tại
    /// → cập nhật thành công, classification được tính lại.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenGradeExists_ShouldReturnUpdatedResponse()
    {
        // Arrange
        var grade = new Grade { Id = 1, StudentId = 10, ClassId = 20, Score = 6.0 };
        _gradeRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(grade);
        _gradeRepo.Setup(r => r.UpdateAsync(It.IsAny<Grade>())).ReturnsAsync(true);
        SetupBuildResponse(studentId: 10, classId: 20, userId: 5);

        var sut = BuildService();

        // Act
        var result = await sut.UpdateAsync(1, new UpdateGradeRequest { Score = 9.5 });

        // Assert
        result.Score.Should().Be(9.5);
        result.Classification.Should().Be("Distinction");
        _gradeRepo.Verify(r => r.UpdateAsync(It.IsAny<Grade>()), Times.Once);
    }

    /// <summary>
    /// Chỉnh sửa điểm không hợp lệ: grade không tồn tại
    /// → ném AppException GRADE_NOT_EXISTED.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenGradeDoesNotExist_ShouldThrowGradeNotExisted()
    {
        // Arrange
        _gradeRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Grade?)null);

        var sut = BuildService();

        // Act
        var act = () => sut.UpdateAsync(999, new UpdateGradeRequest { Score = 8.0 });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.GRADE_NOT_EXISTED);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3. Xếp loại theo thang điểm (Classify)
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Điểm >= 9.0 → Distinction (Xuất sắc).
    /// </summary>
    [Theory]
    [InlineData(9.0)]
    [InlineData(10.0)]
    public void Classify_WhenScoreIsAtOrAbove9_ShouldReturnDistinction(double score)
    {
        GradeService.Classify(score).Should().Be("Distinction");
    }

    /// <summary>
    /// Điểm >= 8.0 và < 9.0 → Merit (Khá).
    /// </summary>
    [Theory]
    [InlineData(8.0)]
    [InlineData(8.9)]
    public void Classify_WhenScoreIsBetween8And9_ShouldReturnMerit(double score)
    {
        GradeService.Classify(score).Should().Be("Merit");
    }

    /// <summary>
    /// Điểm >= 6.5 và < 8.0 → Pass (Trung bình).
    /// </summary>
    [Theory]
    [InlineData(6.5)]
    [InlineData(7.9)]
    public void Classify_WhenScoreIsBetween6Point5And8_ShouldReturnPass(double score)
    {
        GradeService.Classify(score).Should().Be("Pass");
    }

    /// <summary>
    /// Điểm < 6.5 → Refer (Yếu / Thi lại).
    /// </summary>
    [Theory]
    [InlineData(6.4)]
    [InlineData(0.0)]
    public void Classify_WhenScoreIsBelow6Point5_ShouldReturnRefer(double score)
    {
        GradeService.Classify(score).Should().Be("Refer");
    }
}

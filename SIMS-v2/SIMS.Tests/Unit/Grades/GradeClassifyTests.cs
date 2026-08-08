using FluentAssertions;
using Moq;
using SIMS.Application.DTOs.Grades;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;
using Xunit;

namespace SIMS.Tests.Unit.Grades;

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
        _userRepo.Object
    );

    // ── Helpers ────────────────────────────────────────────────────────── //

    private static Enrollment SampleEnrollment(int id = 1, int studentId = 10, int classId = 20) => new()
    {
        Id        = id,
        StudentId = studentId,
        ClassId   = classId
    };

    /// <summary>Một grade record đã nộp bài nhưng chưa được nhập điểm.</summary>
    private static Grade SubmittedGrade(int enrollmentId = 1) => new()
    {
        Id             = 5,
        EnrollmentId   = enrollmentId,
        StudentId      = 10,
        ClassId        = 20,
        SubmissionPath = "files/1_abc.pdf",
        GradedAt       = null,       // chưa nhập điểm
        UpdatedAt      = DateTime.UtcNow
    };

    /// <summary>Một grade record đã được nhập điểm đầy đủ.</summary>
    private static Grade GradedGrade(int id = 5) => new()
    {
        Id             = id,
        EnrollmentId   = 1,
        StudentId      = 10,
        ClassId        = 20,
        Score          = 8.0,
        Classification = "Merit",
        SubmissionPath = "files/1_abc.pdf",
        GradedAt       = DateTime.UtcNow.AddDays(-1),  // đã nhập điểm
        UpdatedAt      = DateTime.UtcNow.AddDays(-1)
    };

    private void SetupBuildResponse(int studentId = 10, int classId = 20, int userId = 5)
    {
        _studentRepo.Setup(r => r.GetByIdAsync(studentId))
                    .ReturnsAsync(new Student { Id = studentId, UserId = userId });
        _classRepo.Setup(r => r.GetByIdAsync(classId))
                  .ReturnsAsync(new Class { Id = classId, ClassCode = "IT101-01" });
        _userRepo.Setup(r => r.GetByIdAsync(userId))
                 .ReturnsAsync(new User { Id = userId, FirstName = "Nguyen", LastName = "Van A" });
    }

    // ══════════════════════════════════════════════════════════════════════
    // 1. Nộp bài (SubmitAsync)
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Lần đầu nộp bài — chưa có grade record nào cho enrollment
    /// → AddAsync được gọi, SubmissionPath được ghi, GradedAt vẫn null.
    /// </summary>
    [Fact]
    public async Task SubmitAsync_WhenFirstSubmission_ShouldCreateGradeRecordWithPath()
    {
        // Arrange
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SampleEnrollment());
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync((Grade?)null);
        _gradeRepo.Setup(r => r.AddAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
        SetupBuildResponse();

        var sut = BuildService();

        // Act
        var result = await sut.SubmitAsync(1, "files/1_xyz.pdf");

        // Assert
        result.SubmissionPath.Should().Be("files/1_xyz.pdf");
        result.GradedAt.Should().BeNull();
        _gradeRepo.Verify(r => r.AddAsync(It.Is<Grade>(g =>
            g.SubmissionPath == "files/1_xyz.pdf" && g.GradedAt == null)), Times.Once);
        _gradeRepo.Verify(r => r.UpdateAsync(It.IsAny<Grade>()), Times.Never);
    }

    /// <summary>
    /// Nộp lại bài — đã có grade record (chưa nhập điểm)
    /// → UpdateAsync được gọi, path được ghi đè, score/GradedAt không thay đổi.
    /// </summary>
    [Fact]
    public async Task SubmitAsync_WhenResubmission_ShouldUpdatePathAndPreserveScore()
    {
        // Arrange
        var existing = SubmittedGrade();
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SampleEnrollment());
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync(existing);
        _gradeRepo.Setup(r => r.UpdateAsync(It.IsAny<Grade>())).ReturnsAsync(true);
        SetupBuildResponse();

        var sut = BuildService();

        // Act
        var result = await sut.SubmitAsync(1, "files/1_new.pdf");

        // Assert
        result.SubmissionPath.Should().Be("files/1_new.pdf");
        _gradeRepo.Verify(r => r.UpdateAsync(It.Is<Grade>(g =>
            g.SubmissionPath == "files/1_new.pdf")), Times.Once);
        _gradeRepo.Verify(r => r.AddAsync(It.IsAny<Grade>()), Times.Never);
    }

    /// <summary>
    /// Nộp bài khi enrollment không tồn tại
    /// → ném AppException ENROLLMENT_NOT_EXISTED.
    /// </summary>
    [Fact]
    public async Task SubmitAsync_WhenEnrollmentDoesNotExist_ShouldThrowEnrollmentNotExisted()
    {
        // Arrange
        _enrollmentRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Enrollment?)null);

        var sut = BuildService();

        // Act
        var act = () => sut.SubmitAsync(99, "files/99_x.pdf");

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.ENROLLMENT_NOT_EXISTED);
        _gradeRepo.Verify(r => r.AddAsync(It.IsAny<Grade>()), Times.Never);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2. Nhập điểm (CreateAsync) — yêu cầu đã nộp bài trước
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Nhập điểm thành công — đã có submission, chưa từng nhập điểm
    /// → UpdateAsync được gọi, GradedAt được gán, score và classification đúng.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenSubmissionExistsAndNotYetGraded_ShouldEnterGrade()
    {
        // Arrange
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SampleEnrollment());
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync(SubmittedGrade());
        _gradeRepo.Setup(r => r.UpdateAsync(It.IsAny<Grade>())).ReturnsAsync(true);
        SetupBuildResponse();

        var sut = BuildService();

        // Act
        var result = await sut.CreateAsync(new CreateGradeRequest { EnrollmentId = 1, Score = 7.5 });

        // Assert
        result.Score.Should().Be(7.5);
        result.Classification.Should().Be("Pass");
        result.GradedAt.Should().NotBeNull();
        _gradeRepo.Verify(r => r.UpdateAsync(It.Is<Grade>(g =>
            g.Score == 7.5 && g.GradedAt != null)), Times.Once);
        _gradeRepo.Verify(r => r.AddAsync(It.IsAny<Grade>()), Times.Never);
    }

    /// <summary>
    /// Nhập điểm thất bại — chưa có bất kỳ grade record nào (chưa nộp bài)
    /// → ném AppException SUBMISSION_NOT_FOUND.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenNoGradeRecord_ShouldThrowSubmissionNotFound()
    {
        // Arrange
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SampleEnrollment());
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync((Grade?)null);

        var sut = BuildService();

        // Act
        var act = () => sut.CreateAsync(new CreateGradeRequest { EnrollmentId = 1, Score = 8.0 });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.SUBMISSION_NOT_FOUND);
        _gradeRepo.Verify(r => r.UpdateAsync(It.IsAny<Grade>()), Times.Never);
    }

    /// <summary>
    /// Nhập điểm thất bại — có grade record nhưng SubmissionPath = null (chưa nộp file)
    /// → ném AppException SUBMISSION_NOT_FOUND.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenSubmissionPathIsNull_ShouldThrowSubmissionNotFound()
    {
        // Arrange
        var gradeWithoutPath = new Grade { Id = 3, EnrollmentId = 1, SubmissionPath = null };
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SampleEnrollment());
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync(gradeWithoutPath);

        var sut = BuildService();

        // Act
        var act = () => sut.CreateAsync(new CreateGradeRequest { EnrollmentId = 1, Score = 8.0 });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.SUBMISSION_NOT_FOUND);
    }

    /// <summary>
    /// Nhập điểm thất bại — đã từng nhập điểm rồi (GradedAt != null)
    /// → ném AppException GRADE_ALREADY_EXISTS.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenGradeAlreadyEntered_ShouldThrowGradeAlreadyExists()
    {
        // Arrange
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SampleEnrollment());
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync(GradedGrade());

        var sut = BuildService();

        // Act
        var act = () => sut.CreateAsync(new CreateGradeRequest { EnrollmentId = 1, Score = 8.0 });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.GRADE_ALREADY_EXISTS);
    }

    /// <summary>
    /// Nhập điểm thất bại — enrollment không tồn tại
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
        _gradeRepo.Verify(r => r.UpdateAsync(It.IsAny<Grade>()), Times.Never);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3. Sửa điểm (UpdateAsync) — yêu cầu đã từng nhập điểm
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Sửa điểm thành công — grade tồn tại và đã được nhập điểm (GradedAt != null)
    /// → UpdateAsync được gọi, điểm và classification mới đúng.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenGradeHasBeenEntered_ShouldReturnUpdatedResponse()
    {
        // Arrange
        _gradeRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(GradedGrade(id: 5));
        _gradeRepo.Setup(r => r.UpdateAsync(It.IsAny<Grade>())).ReturnsAsync(true);
        SetupBuildResponse();

        var sut = BuildService();

        // Act
        var result = await sut.UpdateAsync(5, new UpdateGradeRequest { Score = 9.5 });

        // Assert
        result.Score.Should().Be(9.5);
        result.Classification.Should().Be("Distinction");
        _gradeRepo.Verify(r => r.UpdateAsync(It.Is<Grade>(g =>
            g.Score == 9.5 && g.Classification == "Distinction")), Times.Once);
    }

    /// <summary>
    /// Sửa điểm thất bại — grade record không tồn tại
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

    /// <summary>
    /// Sửa điểm thất bại — grade record tồn tại nhưng GradedAt = null (chưa nhập điểm)
    /// → ném AppException GRADE_NOT_YET_ENTERED.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenGradeNotYetEntered_ShouldThrowGradeNotYetEntered()
    {
        // Arrange — grade record chỉ có submission, chưa có điểm
        _gradeRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(SubmittedGrade());

        var sut = BuildService();

        // Act
        var act = () => sut.UpdateAsync(5, new UpdateGradeRequest { Score = 8.0 });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.GRADE_NOT_YET_ENTERED);
        _gradeRepo.Verify(r => r.UpdateAsync(It.IsAny<Grade>()), Times.Never);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 4. Trạng thái null khi chưa chấm điểm
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Lần đầu nộp bài — Score và Classification phải là null (chưa nhập điểm).
    /// </summary>
    [Fact]
    public async Task SubmitAsync_WhenFirstSubmission_ScoreAndClassificationShouldBeNull()
    {
        // Arrange
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SampleEnrollment());
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync((Grade?)null);
        _gradeRepo.Setup(r => r.AddAsync(It.IsAny<Grade>())).Returns(Task.CompletedTask);
        SetupBuildResponse();

        var sut = BuildService();

        // Act
        var result = await sut.SubmitAsync(1, "files/1_xyz.pdf");

        // Assert
        result.Score.Should().BeNull();
        result.Classification.Should().BeNull();
        result.GradedAt.Should().BeNull();
        result.SubmissionPath.Should().Be("files/1_xyz.pdf");
    }

    /// <summary>
    /// Sau khi nhập điểm (CreateAsync), Score và Classification phải được gán đúng.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenGradeEntered_ScoreAndClassificationShouldNotBeNull()
    {
        // Arrange
        _enrollmentRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(SampleEnrollment());
        _gradeRepo.Setup(r => r.GetByEnrollmentIdAsync(1)).ReturnsAsync(SubmittedGrade());
        _gradeRepo.Setup(r => r.UpdateAsync(It.IsAny<Grade>())).ReturnsAsync(true);
        SetupBuildResponse();

        var sut = BuildService();

        // Act
        var result = await sut.CreateAsync(new CreateGradeRequest { EnrollmentId = 1, Score = 7.5 });

        // Assert
        result.Score.Should().NotBeNull().And.Be(7.5);
        result.Classification.Should().NotBeNullOrEmpty().And.Be("Pass");
        result.GradedAt.Should().NotBeNull();
    }

    /// <summary>
    /// Sau khi sửa điểm (UpdateAsync), Score và Classification phải được cập nhật đúng.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenGradeUpdated_ScoreAndClassificationShouldReflectNewValues()
    {
        // Arrange
        _gradeRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(GradedGrade(id: 5));
        _gradeRepo.Setup(r => r.UpdateAsync(It.IsAny<Grade>())).ReturnsAsync(true);
        SetupBuildResponse();

        var sut = BuildService();

        // Act
        var result = await sut.UpdateAsync(5, new UpdateGradeRequest { Score = 6.0 });

        // Assert
        result.Score.Should().Be(6.0);
        result.Classification.Should().Be("Refer");  // 6.0 < 6.5
    }

    // ══════════════════════════════════════════════════════════════════════
    // 5. Xếp loại điểm số (Classify)
    // ══════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(9.0)]
    [InlineData(10.0)]
    public void Classify_WhenScoreIsAtOrAbove9_ShouldReturnDistinction(double score)
        => GradeService.Classify(score).Should().Be("Distinction");

    [Theory]
    [InlineData(8.0)]
    [InlineData(8.9)]
    public void Classify_WhenScoreIsBetween8And9_ShouldReturnMerit(double score)
        => GradeService.Classify(score).Should().Be("Merit");

    [Theory]
    [InlineData(6.5)]
    [InlineData(7.9)]
    public void Classify_WhenScoreIsBetween6Point5And8_ShouldReturnPass(double score)
        => GradeService.Classify(score).Should().Be("Pass");

    [Theory]
    [InlineData(6.4)]
    [InlineData(0.0)]
    public void Classify_WhenScoreIsBelow6Point5_ShouldReturnRefer(double score)
        => GradeService.Classify(score).Should().Be("Refer");
}

using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SIMS.Application.DTOs.Auth;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Application.Services;
using SIMS.Application.Settings;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;
using Xunit;

namespace SIMS.Tests.Auth;

public class AuthServiceTests
{
    // ── Mocks ──────────────────────────────────────────────────────────── //

    private readonly Mock<IUserRepository>         _userRepo     = new();
    private readonly Mock<IRoleRepository>         _roleRepo     = new();
    private readonly Mock<IPermissionRepository>   _permRepo     = new();
    private readonly Mock<IPasswordHasher>         _hasher       = new();
    private readonly Mock<ITokenService>           _tokenService = new();
    private readonly Mock<IRevokedTokenRepository> _revokedRepo  = new();

    private AuthService BuildService()
    {
        var policy = Options.Create(new TokenPolicy { RefreshWindowMinutes = 60 });
        return new AuthService(
            _userRepo.Object,
            _roleRepo.Object,
            _permRepo.Object,
            _hasher.Object,
            _tokenService.Object,
            _revokedRepo.Object,
            policy);
    }

    // ── Test 1 – Đăng nhập thành công ─────────────────────────────────── //

    /// <summary>
    /// Username hợp lệ (≥ 6 ký tự) + password đúng (≥ 8 ký tự)
    /// → trả về LoginResponse chứa AccessToken.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ShouldReturnAccessToken()
    {
        // Arrange
        // username = "validuser" (9 ký tự, thoả ≥ 6)
        // password = "correctPass1" (12 ký tự, thoả ≥ 8)
        var user = new User
        {
            Id           = 1,
            Username     = "validuser",
            PasswordHash = "hashed",
            Salt         = "salt",
            RoleId       = 1,
            IsActive     = true
        };

        _userRepo.Setup(r => r.GetByUsernameAsync("validuser")).ReturnsAsync(user);
        _hasher.Setup(h => h.VerifyPassword("correctPass1", user.PasswordHash, user.Salt)).Returns(true);
        _roleRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Role { Id = 1, Name = "Student" });
        _permRepo.Setup(r => r.GetByRoleIdAsync(1)).ReturnsAsync([]);
        _tokenService
            .Setup(t => t.GenerateToken(user, "Student", It.IsAny<IEnumerable<string>>()))
            .Returns(new TokenResult("access-token-abc", DateTime.UtcNow.AddHours(1)));

        var sut = BuildService();

        // Act
        var result = await sut.LoginAsync(new LoginRequest
        {
            Username = "validuser",
            Password = "correctPass1"
        });

        // Assert
        result.AccessToken.Should().Be("access-token-abc");
    }

    // ── Test 2 – Username không hợp lệ ────────────────────────────────── //

    /// <summary>
    /// Username quá ngắn (< 6 ký tự) nên không tồn tại trong hệ thống
    /// → ném AppException INVALID_CREDENTIALS, dù password có đúng đi nữa.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WhenUsernameIsNotFound_ShouldThrowInvalidCredentials()
    {
        // Arrange
        // username = "abc" (3 ký tự, vi phạm quy tắc ≥ 6) → không tìm thấy user
        _userRepo.Setup(r => r.GetByUsernameAsync("abc")).ReturnsAsync((User?)null);

        var sut = BuildService();

        // Act
        var act = () => sut.LoginAsync(new LoginRequest
        {
            Username = "abc",
            Password = "correctPass1"
        });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.INVALID_CREDENTIALS);
    }

    // ── Test 3 – Password sai ──────────────────────────────────────────── //

    /// <summary>
    /// Username hợp lệ (≥ 6 ký tự) nhưng password sai
    /// → ném AppException INVALID_CREDENTIALS.
    /// </summary>
    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ShouldThrowInvalidCredentials()
    {
        // Arrange
        // username = "validuser" (9 ký tự, thoả ≥ 6)
        // password = "wrongPass9" → hasher trả về false
        var user = new User
        {
            Id           = 1,
            Username     = "validuser",
            PasswordHash = "hashed",
            Salt         = "salt",
            RoleId       = 1,
            IsActive     = true
        };

        _userRepo.Setup(r => r.GetByUsernameAsync("validuser")).ReturnsAsync(user);
        _hasher.Setup(h => h.VerifyPassword("wrongPass9", user.PasswordHash, user.Salt)).Returns(false);

        var sut = BuildService();

        // Act
        var act = () => sut.LoginAsync(new LoginRequest
        {
            Username = "validuser",
            Password = "wrongPass9"
        });

        // Assert
        await act.Should().ThrowAsync<AppException>()
                 .Where(e => e.ErrorCode == ErrorCode.INVALID_CREDENTIALS);
    }
}

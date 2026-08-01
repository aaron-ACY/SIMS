using FluentAssertions;
using Moq;
using SIMS.Application.DTOs.Auth;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Application.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;
using Xunit;

namespace SIMS.Tests.Auth;

public class AuthServiceTests
{
    // ── Mocks ──────────────────────────────────────────────────────────── //

    private readonly Mock<IUserRepository>          _userRepo          = new();
    private readonly Mock<IRoleRepository>          _roleRepo          = new();
    private readonly Mock<IPermissionRepository>    _permRepo          = new();
    private readonly Mock<IPasswordHasher>          _hasher            = new();
    private readonly Mock<ITokenService>            _tokenService      = new();
    private readonly Mock<ITokenRevocationService>  _revocationService = new();

    private AuthService BuildService() =>
        new(_userRepo.Object,
            _roleRepo.Object,
            _permRepo.Object,
            _hasher.Object,
            _tokenService.Object,
            _revocationService.Object);

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ShouldReturnAccessToken()
    {
        // Arrange
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

    [Fact]
    public async Task LoginAsync_WhenUsernameIsNotFound_ShouldThrowInvalidCredentials()
    {
        // Arrange
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

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ShouldThrowInvalidCredentials()
    {
        // Arrange
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

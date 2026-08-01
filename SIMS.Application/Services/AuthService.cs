using SIMS.Application.DTOs.Auth;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

/// <summary>
/// Handles the three authentication flows: login, logout, and token refresh.
/// Token-revocation details (block-window calculation, persistence) are
/// delegated entirely to <see cref="ITokenRevocationService"/>.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository          _userRepository;
    private readonly IRoleRepository          _roleRepository;
    private readonly IPermissionRepository    _permissionRepository;
    private readonly IPasswordHasher          _passwordHasher;
    private readonly ITokenService            _tokenService;
    private readonly ITokenRevocationService  _revocationService;

    public AuthService(
        IUserRepository         userRepository,
        IRoleRepository         roleRepository,
        IPermissionRepository   permissionRepository,
        IPasswordHasher         passwordHasher,
        ITokenService           tokenService,
        ITokenRevocationService revocationService)
    {
        _userRepository       = userRepository;
        _roleRepository       = roleRepository;
        _permissionRepository = permissionRepository;
        _passwordHasher       = passwordHasher;
        _tokenService         = tokenService;
        _revocationService    = revocationService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var username = request.Username.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByUsernameAsync(username);

        // Single generic error prevents username-enumeration attacks.
        if (user is null || !user.IsActive)
            throw new AppException(ErrorCode.INVALID_CREDENTIALS);

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
            throw new AppException(ErrorCode.INVALID_CREDENTIALS);

        var role = await _roleRepository.GetByIdAsync(user.RoleId)
                   ?? throw new AppException(ErrorCode.INVALID_CREDENTIALS);

        var permissions     = await _permissionRepository.GetByRoleIdAsync(user.RoleId);
        var permissionNames = permissions.Select(p => p.Name);

        var tokenResult = _tokenService.GenerateToken(user, role.Name, permissionNames);

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt   = tokenResult.ExpiresAt
        };
    }

    public async Task LogoutAsync(string rawToken, CancellationToken ct = default)
    {
        // Full validation: signature, issuer and audience are all verified.
        // Lifetime is intentionally skipped — a just-expired token must still
        // be revocable so the user can log out cleanly after a brief network
        // delay. GetJtiFromToken / GetExpiryFromToken only parse (no signature
        // check), so they must not be used here.
        var principal = _tokenService.ValidateIgnoringLifetime(rawToken)
                        ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        await _revocationService.RevokeAsync(
            principal.Jti, principal.UserId, principal.ExpiresAt, ct);
    }

    public async Task<LoginResponse> RefreshTokenAsync(
        RefreshTokenRequest request, CancellationToken ct = default)
    {
        // Verifies signature, issuer and audience; skips only the lifetime check.
        var principal = _tokenService.ValidateIgnoringLifetime(request.AccessToken)
                        ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        // Catches both logout and a previous refresh of this same token.
        if (await _revocationService.IsRevokedAsync(principal.Jti, ct))
            throw new AppException(ErrorCode.INVALID_TOKEN);

        var user = await _userRepository.GetByIdAsync(principal.UserId)
                   ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        // A deactivated account must not be able to refresh its way back in.
        if (!user.IsActive)
            throw new AppException(ErrorCode.INVALID_TOKEN);

        var role = await _roleRepository.GetByIdAsync(user.RoleId)
                   ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        // Re-read from the store rather than copying the old token's claims, so a
        // permission revoked since login does not survive by being carried forward.
        var permissions = await _permissionRepository.GetByRoleIdAsync(user.RoleId);

        var tokenResult = _tokenService.GenerateToken(
            user, role.Name, permissions.Select(p => p.Name));

        // Rotate: burn the presented token so it cannot be replayed or refreshed twice.
        // Done after the new token is minted so a failure here cannot strand the caller
        // without either token.
        await _revocationService.RevokeAsync(
            principal.Jti, user.Id, principal.ExpiresAt, ct);

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt   = tokenResult.ExpiresAt
        };
    }
}

using Microsoft.Extensions.Options;
using SIMS.Application.DTOs.Auth;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Application.Settings;
using SIMS.Domain.Entities;
using SIMS.Shared.Exceptions;

namespace SIMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRevokedTokenRepository _revokedTokenRepository;
    private readonly TokenPolicy _tokenPolicy;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRevokedTokenRepository revokedTokenRepository,
        IOptions<TokenPolicy> tokenPolicy)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _revokedTokenRepository = revokedTokenRepository;
        _tokenPolicy = tokenPolicy.Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
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

        var permissions    = await _permissionRepository.GetByRoleIdAsync(user.RoleId);
        var permissionNames = permissions.Select(p => p.Name);

        var tokenResult = _tokenService.GenerateToken(user, role.Name, permissionNames);

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken
        };
    }

    public async Task LogoutAsync(string rawToken, int userId)
    {
        var jti = _tokenService.GetJtiFromToken(rawToken)
                  ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        var expiry = _tokenService.GetExpiryFromToken(rawToken) ?? DateTime.UtcNow;

        await RevokeAsync(jti, userId, expiry);
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Verifies signature, issuer and audience; skips only the lifetime check.
        var principal = _tokenService.ValidateIgnoringLifetime(request.AccessToken)
                        ?? throw new AppException(ErrorCode.INVALID_TOKEN);

        // Bound how stale a token may be. Without this, any token ever issued could be
        // traded for a live one indefinitely, which would make expiry meaningless.
        var refreshDeadline = principal.ExpiresAt.AddMinutes(_tokenPolicy.RefreshWindowMinutes);
        if (DateTime.UtcNow > refreshDeadline)
            throw new AppException(ErrorCode.REFRESH_WINDOW_EXPIRED);

        // Catches both logout and a previous refresh of this same token.
        if (await _revokedTokenRepository.IsRevokedAsync(principal.Jti))
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
        await RevokeAsync(principal.Jti, user.Id, principal.ExpiresAt);

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken
        };
    }

    // ------------------------------------------------------------------ //

    /// <summary>
    /// Records a token as revoked.
    ///
    /// The stored ExpiresAt is pushed out to the end of the refresh window rather than
    /// the token's own expiry. IsRevokedAsync ignores entries whose ExpiresAt has passed
    /// (and RevokeAsync prunes them), so an entry kept only until the token's own expiry
    /// would stop blocking exactly when the refresh endpoint still needs it — letting a
    /// logged-out token be exchanged for a live one once it aged past exp.
    /// </summary>
    private async Task RevokeAsync(string jti, int userId, DateTime tokenExpiry)
    {
        var blockUntil = tokenExpiry.AddMinutes(_tokenPolicy.RefreshWindowMinutes);

        // Past the refresh window the token is inert for every path, so there is
        // nothing left to guard against.
        if (blockUntil <= DateTime.UtcNow)
            return;

        await _revokedTokenRepository.RevokeAsync(new RevokedToken
        {
            Jti       = jti,
            UserId    = userId,
            RevokedAt = DateTime.UtcNow,
            ExpiresAt = blockUntil
        });
    }
}

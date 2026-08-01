using SIMS.Application.DTOs.Auth;

namespace SIMS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Revokes the supplied JWT so it is rejected on all future requests.
    /// The token is fully validated (signature, issuer, audience) while
    /// tolerating expiry — a just-expired token must still be revocable.
    /// Returns normally when the token is already revoked (idempotent).
    /// Throws <see cref="SIMS.Shared.Exceptions.AppException"/> with
    /// <c>INVALID_TOKEN</c> when the token is malformed or its signature
    /// cannot be verified.
    /// </summary>
    Task LogoutAsync(string rawToken, CancellationToken ct = default);

    /// <summary>
    /// Exchanges an access token — expired or not — for a freshly signed one.
    /// The old token is revoked so it cannot be replayed or refreshed twice.
    /// Role and permissions are re-read from the store, so changes made since the
    /// original login take effect immediately instead of being carried forward.
    /// </summary>
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
}

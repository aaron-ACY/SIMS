using SIMS.Application.DTOs.Auth;

namespace SIMS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Registers a new user account linked to an existing student or instructor profile.
    /// The supplied email must already appear in the students or instructors import list;
    /// the backend resolves the role and links the new account to the matching profile.
    /// Throws <see cref="SIMS.Shared.Exceptions.AppException"/> with
    /// <c>EMAIL_NOT_REGISTERED</c> when the email is not found, or
    /// <c>ACCOUNT_ALREADY_LINKED</c> when an account already exists for that profile.
    /// Returns a signed JWT so the caller is logged in immediately after registering.
    /// </summary>
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

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

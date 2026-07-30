using SIMS.Application.DTOs.Auth;

namespace SIMS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Revokes the supplied JWT so it is rejected on all future requests.
    /// userId is taken from the already-validated claims in the controller
    /// to avoid re-parsing the token for the subject.
    /// </summary>
    Task LogoutAsync(string rawToken, int userId);

    /// <summary>
    /// Exchanges an access token — expired or not — for a freshly signed one.
    /// The old token is revoked so it cannot be replayed or refreshed twice.
    /// Role and permissions are re-read from the store, so changes made since the
    /// original login take effect immediately instead of being carried forward.
    /// </summary>
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
}

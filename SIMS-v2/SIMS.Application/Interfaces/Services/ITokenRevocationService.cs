namespace SIMS.Application.Interfaces.Services;

/// <summary>
/// Owns the token-revocation concern: persisting revocations and checking
/// whether a given JTI is currently on the block-list.
///
/// Extracted from <see cref="SIMS.Application.Services.AuthService"/> so that
/// AuthService can focus on authentication flows (login / logout / refresh) while
/// this service owns the revocation policy (how long a revocation record lives,
/// the relationship with the refresh window, pruning, etc.).
/// </summary>
public interface ITokenRevocationService
{
    /// <summary>
    /// Blocks the token identified by <paramref name="jti"/> until the end of the
    /// refresh window so a logged-out or already-refreshed token cannot be traded
    /// in at <c>/api/auth/refresh</c>.
    /// No-op when the token is already past the refresh window — there is nothing
    /// left to block at that point.
    /// </summary>
    Task RevokeAsync(string jti, int userId, DateTime tokenExpiry,
                     CancellationToken ct = default);

    /// <summary>
    /// Returns <c>true</c> when the given JTI appears in the active revocation
    /// list (i.e. it has been revoked and the block window has not yet lapsed).
    /// </summary>
    Task<bool> IsRevokedAsync(string jti, CancellationToken ct = default);
}

namespace SIMS.Application.DTOs.Auth;

/// <summary>
/// The trustworthy parts of a signature-verified token whose lifetime was not checked.
/// Deliberately carries only identity and timing — the role and permission claims from
/// the old token are not surfaced, because refresh re-reads them from the store so a
/// revoked permission cannot survive by being copied forward.
/// </summary>
/// <param name="UserId">Value of the sub claim.</param>
/// <param name="Jti">Value of the jti claim, used for the revocation check.</param>
/// <param name="ExpiresAt">Value of the exp claim, in UTC.</param>
public record ExpiredTokenPrincipal(int UserId, string Jti, DateTime ExpiresAt);

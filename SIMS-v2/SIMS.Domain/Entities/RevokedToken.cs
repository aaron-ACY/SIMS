namespace SIMS.Domain.Entities;

/// <summary>
/// Records a JWT that has been explicitly revoked via logout.
/// The JTI (JWT ID) is checked on every authenticated request.
/// </summary>
public class RevokedToken
{
    public int Id { get; set; }

    /// <summary>JWT ID claim (jti) — uniquely identifies the token.</summary>
    public string Jti { get; set; } = string.Empty;

    public int UserId { get; set; }

    public DateTime RevokedAt { get; set; }

    /// <summary>
    /// When this revocation record stops mattering: the token's own expiry
    /// plus the refresh window. It is NOT the token's <c>exp</c> claim — an
    /// expired token can still be traded in at /api/auth/refresh, so the
    /// record has to outlive <c>exp</c> or a logged-out token would become
    /// refreshable again. Entries past this date are inert and get pruned.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

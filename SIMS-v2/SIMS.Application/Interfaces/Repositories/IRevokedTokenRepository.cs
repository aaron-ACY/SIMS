using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Repositories;

public interface IRevokedTokenRepository
{
    /// <summary>Returns true if the given JTI is in the revocation list and has not expired.</summary>
    Task<bool> IsRevokedAsync(string jti);

    /// <summary>Persists a new revocation record (and prunes stale entries).</summary>
    Task RevokeAsync(RevokedToken token);
}

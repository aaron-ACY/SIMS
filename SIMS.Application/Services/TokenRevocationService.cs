using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Application.Settings;
using SIMS.Domain.Entities;

namespace SIMS.Application.Services;

/// <summary>
/// Handles token revocation: persisting block records and enforcing the
/// refresh-window policy that determines how long a revocation must be kept.
///
/// Extracted from AuthService to respect the Single Responsibility Principle —
/// AuthService now delegates the entire revocation concern here.
/// </summary>
public sealed class TokenRevocationService : ITokenRevocationService
{
    private readonly IRevokedTokenRepository _repository;
    private readonly TokenPolicy             _policy;

    public TokenRevocationService(
        IRevokedTokenRepository   repository,
        IOptions<TokenPolicy>     tokenPolicy)
    {
        _repository = repository;
        _policy     = tokenPolicy.Value;
    }

    /// <inheritdoc/>
    public async Task RevokeAsync(string jti, int userId, DateTime tokenExpiry,
                                  CancellationToken ct = default)
    {
        // Push the block window out to the end of the refresh window, not just to
        // the token's own expiry.  Without this, a logged-out token would become
        // refreshable again once exp passes but the refresh window hasn't closed.
        var blockUntil = tokenExpiry.AddMinutes(_policy.RefreshWindowMinutes);

        // Already past the refresh window — the token is inert on every path.
        if (blockUntil <= DateTime.UtcNow)
            return;

        await _repository.RevokeAsync(new RevokedToken
        {
            Jti       = jti,
            UserId    = userId,
            RevokedAt = DateTime.UtcNow,
            ExpiresAt = blockUntil
        });
    }

    /// <inheritdoc/>
    public Task<bool> IsRevokedAsync(string jti, CancellationToken ct = default)
        => _repository.IsRevokedAsync(jti);
}

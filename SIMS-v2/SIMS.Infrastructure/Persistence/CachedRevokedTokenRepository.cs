using Microsoft.Extensions.Caching.Memory;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;

namespace SIMS.Infrastructure.Persistence;

/// <summary>
/// Decorator that adds an in-process memory cache in front of
/// <see cref="RevokedTokenRepository"/>.
///
/// <para>
/// <see cref="IsRevokedAsync"/> is called on every authenticated request
/// inside <c>OnTokenValidated</c>. Without a cache layer each call opens and
/// parses <c>revoked_tokens.csv</c> from disk.  This decorator short-circuits
/// that read for JTIs already seen within the current TTL window.
/// </para>
///
/// <para>Cache strategy:</para>
/// <list type="bullet">
///   <item><term>Miss</term><description>
///     Delegates to the inner repository, then caches the result.
///     A <c>false</c> (not revoked) result is cached for
///     <see cref="NegativeTtl"/> — short enough that a revocation issued on
///     another code path becomes visible quickly.
///     A <c>true</c> (revoked) result is cached for <see cref="PositiveTtl"/>
///     — once revoked a token never becomes valid again.
///   </description></item>
///   <item><term><see cref="RevokeAsync"/></term><description>
///     Writes the revocation to the inner repository, then immediately
///     inserts a <c>true</c> entry into the cache so any in-flight request
///     using the same JTI is rejected without waiting for the TTL to lapse.
///   </description></item>
/// </list>
/// </summary>
public sealed class CachedRevokedTokenRepository : IRevokedTokenRepository
{
    // "not-revoked" entries are re-validated after this window.
    // Within a single process the window is irrelevant for tokens the current
    // instance revoked (RevokeAsync writes true immediately), but it bounds the
    // stale-read risk for any external mutation of the CSV.
    private static readonly TimeSpan NegativeTtl = TimeSpan.FromMinutes(1);

    // Revoked entries are safe to keep longer — the decision is irreversible.
    private static readonly TimeSpan PositiveTtl = TimeSpan.FromMinutes(30);

    private readonly IRevokedTokenRepository _inner;
    private readonly IMemoryCache            _cache;

    public CachedRevokedTokenRepository(
        IRevokedTokenRepository inner,
        IMemoryCache            cache)
    {
        _inner = inner;
        _cache = cache;
    }

    /// <inheritdoc/>
    public async Task<bool> IsRevokedAsync(string jti)
    {
        var key = CacheKey(jti);

        if (_cache.TryGetValue(key, out bool cached))
            return cached;

        var revoked = await _inner.IsRevokedAsync(jti);

        _cache.Set(key, revoked, revoked ? PositiveTtl : NegativeTtl);
        return revoked;
    }

    /// <inheritdoc/>
    public async Task RevokeAsync(RevokedToken token)
    {
        await _inner.RevokeAsync(token);

        // Immediately mark this JTI as revoked in the cache so subsequent
        // IsRevokedAsync calls within the same process don't need a CSV read.
        _cache.Set(CacheKey(token.Jti), true, PositiveTtl);
    }

    private static string CacheKey(string jti) => $"revoked:{jti}";
}

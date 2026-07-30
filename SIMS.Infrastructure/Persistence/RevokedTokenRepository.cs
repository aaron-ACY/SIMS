using Microsoft.Extensions.Options;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Persistence.Base;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Persistence;

public class RevokedTokenRepository : CsvRepositoryBase<RevokedToken>, IRevokedTokenRepository
{
    public RevokedTokenRepository(IOptions<DataStoreSettings> settings)
        : base(settings.Value.ResolvePath("revoked_tokens.csv")) { }

    public async Task<bool> IsRevokedAsync(string jti)
    {
        var tokens = await ReadAllAsync();
        // Only entries that haven't expired yet count — expired tokens are
        // harmless since the JWT middleware already rejects them on lifetime.
        return tokens.Any(t =>
            string.Equals(t.Jti, jti, StringComparison.Ordinal) &&
            t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task RevokeAsync(RevokedToken token)
    {
        var tokens = await ReadAllAsync();

        // Prune expired entries on every write to keep the file lean.
        var active = tokens
            .Where(t => t.ExpiresAt > DateTime.UtcNow)
            .ToList();

        token.Id = active.Count == 0 ? 1 : active.Max(t => t.Id) + 1;
        active.Add(token);

        await WriteAllAsync(active);
    }
}

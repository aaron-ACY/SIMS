using SIMS.Application.DTOs.Auth;
using SIMS.Domain.Entities;

namespace SIMS.Application.Interfaces.Services;

public interface ITokenService
{
    /// <summary>Generates a signed JWT for the authenticated user.</summary>
    TokenResult GenerateToken(User user, string roleName, IEnumerable<string> permissions);

    /// <summary>Extracts the JTI (JWT ID) claim from a raw token string without validating signature.</summary>
    string? GetJtiFromToken(string token);

    /// <summary>Extracts the expiry (exp) from a raw token string without validating signature.</summary>
    DateTime? GetExpiryFromToken(string token);

    /// <summary>
    /// Fully validates a token's signature, issuer and audience while deliberately
    /// skipping the lifetime check, so an expired token can still be exchanged for a
    /// new one. Returns null when the token is malformed, unsigned, tampered with, or
    /// signed by a different key. The caller is responsible for enforcing its own
    /// bound on how long past expiry a token remains acceptable.
    /// </summary>
    ExpiredTokenPrincipal? ValidateIgnoringLifetime(string token);
}

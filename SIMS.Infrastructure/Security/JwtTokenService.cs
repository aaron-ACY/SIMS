using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SIMS.Application.DTOs.Auth;
using SIMS.Application.Interfaces.Services;
using SIMS.Domain.Constants;
using SIMS.Domain.Entities;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public TokenResult GenerateToken(User user, string roleName, IEnumerable<string> permissions)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);
        var jti    = Guid.NewGuid().ToString();

        var header = new JwtHeader(creds);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(CustomClaimTypes.Role,       roleName),
        };

        foreach (var permission in permissions)
            claims.Add(new Claim(CustomClaimTypes.Permission, permission));

        var payload = new JwtPayload(
            issuer:    _settings.Issuer,
            audience:  _settings.Audience,
            claims:    claims,
            notBefore: null,   
            expires:   expiry);

        var token = new JwtSecurityToken(header, payload);
        return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }

    public string? GetJtiFromToken(string token)
    {
        var jwt = ParseToken(token);
        return jwt?.Id;
    }

    public DateTime? GetExpiryFromToken(string token)
    {
        var jwt = ParseToken(token);
        return jwt?.ValidTo;
    }

    public ExpiredTokenPrincipal? ValidateIgnoringLifetime(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = _settings.Issuer,
            ValidAudience            = _settings.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(_settings.SecretKey)),

            // The whole point of this method: accept an expired token so it can be
            // traded in. Every other check still applies, and AuthService bounds how
            // far past expiry is acceptable.
            ValidateLifetime = false,

            // Keep raw claim names ("sub", "jti") consistent with the rest of the app.
            RoleClaimType = CustomClaimTypes.Role
        };

        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(token, parameters, out var validated);

            // Reject anything not signed with the expected symmetric algorithm — guards
            // against an "alg: none" or algorithm-substitution token slipping through.
            if (validated is not JwtSecurityToken jwt ||
                !string.Equals(jwt.Header.Alg, SecurityAlgorithms.HmacSha256,
                               StringComparison.Ordinal))
            {
                return null;
            }

            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (!int.TryParse(sub, out var userId) || string.IsNullOrWhiteSpace(jti))
                return null;

            return new ExpiredTokenPrincipal(userId, jti, jwt.ValidTo);
        }
        catch
        {
            // Malformed, tampered, wrong key, wrong issuer/audience.
            return null;
        }
    }

    // ------------------------------------------------------------------ //

    private static JwtSecurityToken? ParseToken(string token)
    {
        try
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch
        {
            return null;
        }
    }
}

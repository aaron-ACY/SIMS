namespace SIMS.Domain.Constants;

/// <summary>
/// Non-standard JWT claim names issued by this API. Kept in one place so the
/// token writer (JwtTokenService) and the readers (authorization policies,
/// RoleClaimType config) cannot drift apart.
/// </summary>
public static class CustomClaimTypes
{
    public const string Role       = "role";
    public const string Permission = "permission";
}

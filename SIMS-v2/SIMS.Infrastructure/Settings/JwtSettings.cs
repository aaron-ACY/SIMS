namespace SIMS.Infrastructure.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 30;

    /// <summary>
    /// How long after an access token expires it may still be exchanged for a new one
    /// via POST /api/auth/refresh. Bounds how long a leaked token stays useful — past
    /// this window the user must log in again. Defaults to 7 days.
    /// </summary>
    public int RefreshWindowMinutes { get; set; } = 10080;
}

namespace SIMS.Application.Settings;

/// <summary>
/// Token rules the Application layer needs to enforce. Kept separate from
/// Infrastructure's JwtSettings (signing key, issuer, audience) because Application
/// cannot reference Infrastructure — DI binds this from the same "Jwt" config section.
/// </summary>
public class TokenPolicy
{
    /// <summary>
    /// How long after an access token expires it may still be exchanged for a new one.
    /// Bounds how long a leaked token stays useful.
    /// </summary>
    public int RefreshWindowMinutes { get; set; } = 10080;
}

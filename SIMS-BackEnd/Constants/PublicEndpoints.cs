namespace SIMS_BackEnd.Constants;

/// <summary>
/// Endpoints reachable without a JWT. Program.cs walks every mapped route at
/// startup and attaches [AllowAnonymous] to the ones listed here; everything
/// else falls back to requiring an authenticated user.
///
/// Keep this list as small as it can possibly be — each entry is an opening in
/// the authentication wall, so an over-broad pattern here silently unprotects
/// real endpoints. Paths are matched against the route template, not the
/// incoming URL, so they must be written exactly as the controller declares
/// them (no query string, no trailing slash).
/// </summary>
public static class PublicEndpoints
{
    /// <summary>Issues the first token, so it cannot require one.</summary>
    public const string Login = "/api/auth/login";

    /// <summary>
    /// Self-service registration: the caller supplies a pre-loaded Gmail and receives
    /// a JWT on success. Intentionally public — users have no token before they register.
    /// </summary>
    public const string Register = "/api/auth/register";

    /// <summary>
    /// Takes an expired token, which [Authorize] would reject before the
    /// handler ran. Still fully validated inside AuthService.
    /// </summary>
    public const string Refresh = "/api/auth/refresh";

    /// <summary>
    /// Logout is intentionally anonymous so a caller can still revoke their
    /// token even if the JwtBearer middleware rejects it (e.g. already expired
    /// but within the refresh window). The handler itself validates the token
    /// and extracts the user ID — a missing or malformed Authorization header
    /// returns 401 from inside the action.
    /// </summary>
    public const string Logout = "/api/auth/logout";

    /// <summary>
    /// The whitelist itself. A trailing "/*" makes an entry a prefix match
    /// covering everything below that segment; anything else is an exact match.
    ///
    /// Swagger is absent on purpose: it is served by middleware registered
    /// before UseAuthentication(), so it never reaches the authorization stage
    /// and does not need an entry here.
    /// </summary>
    public static readonly string[] All =
    [
        Login,
        Register,
        Refresh,
        Logout
    ];

    /// <summary>
    /// True when <paramref name="routeTemplate"/> is on the whitelist. Accepts
    /// templates with or without a leading slash, and ignores case since route
    /// matching is case-insensitive.
    /// </summary>
    public static bool IsPublic(string? routeTemplate)
    {
        if (string.IsNullOrWhiteSpace(routeTemplate))
            return false;

        var path = routeTemplate.StartsWith('/') ? routeTemplate : "/" + routeTemplate;

        foreach (var entry in All)
        {
            var isMatch = entry.EndsWith("/*", StringComparison.Ordinal)
                ? path.StartsWith(entry[..^1], StringComparison.OrdinalIgnoreCase)
                : path.Equals(entry, StringComparison.OrdinalIgnoreCase);

            if (isMatch)
                return true;
        }

        return false;
    }
}

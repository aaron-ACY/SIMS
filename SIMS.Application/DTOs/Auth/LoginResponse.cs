namespace SIMS.Application.DTOs.Auth;

public class LoginResponse
{
    public string   AccessToken { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the access token expires.</summary>
    public DateTime ExpiresAt   { get; set; }
}

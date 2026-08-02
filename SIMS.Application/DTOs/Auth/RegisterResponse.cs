namespace SIMS.Application.DTOs.Auth;

/// <summary>Returned after a successful self-service registration. Includes a token so the caller is logged in immediately.</summary>
public class RegisterResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    /// <summary>Role assigned: "Student" or "Instructor".</summary>
    public string Role { get; set; } = string.Empty;
}

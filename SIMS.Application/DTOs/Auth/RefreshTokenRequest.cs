using System.ComponentModel.DataAnnotations;

namespace SIMS.Application.DTOs.Auth;

/// <summary>Payload for POST /api/auth/refresh.</summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// The current access token. May be expired, but must carry a valid signature and
    /// still fall inside the configured refresh window.
    /// </summary>
    [Required(ErrorMessage = "AccessToken is required.")]
    public string AccessToken { get; set; } = string.Empty;
}

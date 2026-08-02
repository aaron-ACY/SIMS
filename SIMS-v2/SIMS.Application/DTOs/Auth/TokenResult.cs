namespace SIMS.Application.DTOs.Auth;

/// <summary>Returned by ITokenService.GenerateToken — carries the signed JWT and its expiry.</summary>
public record TokenResult(string AccessToken, DateTime ExpiresAt);

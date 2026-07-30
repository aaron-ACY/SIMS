using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Application.DTOs.Auth;
using SIMS.Application.Interfaces.Services;
using SIMS.Shared.Exceptions;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Authenticates a user and returns a signed JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>),       StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    /// <summary>
    /// Exchanges an access token for a freshly signed one.
    ///
    /// Deliberately anonymous: the token is expected to be expired by the time a client
    /// calls this, and [Authorize] would reject it before the handler ran. The token is
    /// still fully validated inside the service — signature, issuer, audience, revocation
    /// and a bounded staleness window — so this is not an unauthenticated entry point.
    /// The presented token is revoked on success, so each one refreshes at most once.
    ///
    /// Also listed in PublicEndpoints.All, which is what actually exempts it from the
    /// fallback authentication policy. The attribute is kept so the exemption is visible
    /// when reading this controller on its own.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    /// <summary>
    /// Invalidates the caller's current JWT.
    /// The token is added to the revocation list — all subsequent requests
    /// with the same token will receive 401 Unauthorized.
    ///
    /// Deliberately anonymous (listed in PublicEndpoints.All): a token that has just
    /// expired should still be revocable. If [Authorize] were present the middleware
    /// would reject it before the handler ran and the user could not log out cleanly.
    /// The handler itself extracts and validates the token from the Authorization
    /// header and returns 401 when it is missing or malformed.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        // Extract the raw token from the Authorization header.
        var rawToken = HttpContext.Request.Headers.Authorization
            .ToString()
            .Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        // sub claim holds the user ID (claim mapping is disabled — see Program.cs).
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            throw new AppException(ErrorCode.INVALID_TOKEN, "Invalid token claims.");

        await _authService.LogoutAsync(rawToken, userId);

        return Ok(ApiResponse.Ok("Logged out successfully."));
    }
}

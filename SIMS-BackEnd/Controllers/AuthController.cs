using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>),       StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>),       StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    /// <summary>
    /// Registers a new account for a student or instructor whose Gmail was pre-loaded
    /// by an admin. The backend looks up the email in the imported student/instructor
    /// profiles, assigns the matching role, creates the account and returns a JWT so the
    /// caller is logged in immediately.
    ///
    /// Fails with 422 when the email is not found in any pre-loaded profile.
    /// Fails with 400 when an account already exists for this email or username.
    /// </summary>
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request, ct);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<RegisterResponse>.Ok(result, "Account created successfully."));
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
    /// fallback authentication policy.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(request, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    /// <summary>
    /// Invalidates the caller's current JWT.
    /// The token is added to the revocation list — all subsequent requests
    /// with the same token will receive 401 Unauthorized.
    ///
    /// Deliberately anonymous: a token that has just expired should still be revocable.
    /// The handler validates the token's signature, issuer and audience while tolerating
    /// expiry, then returns 401 when the token is missing, malformed, or cannot be verified.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var rawToken = HttpContext.Request.Headers.Authorization
            .ToString()
            .Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        await _authService.LogoutAsync(rawToken);

        return Ok(ApiResponse.Ok("Logged out successfully."));
    }
}

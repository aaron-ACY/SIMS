using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SIMS.Domain.Constants;

namespace SIMS_BackEnd.Authorization;

/// <summary>
/// Dynamically builds an authorization policy for any permission name,
/// instead of pre-registering every known permission at startup.
///
/// ASP.NET Core calls <see cref="GetPolicyAsync"/> at request time when it
/// encounters an <c>[Authorize(Policy = "…")]</c> attribute. This provider
/// returns a policy that requires a matching "permission" claim in the JWT,
/// regardless of whether the permission existed when the app started.
///
/// Benefits over the static <c>foreach (Permissions.All)</c> approach:
/// • Permissions created via the API work immediately — no restart required.
/// • The list of known permissions lives only in permissions.csv; the C# code
///   does not need to be updated when a new permission is created at runtime.
/// </summary>
public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        // Delegate default/fallback policy to the built-in provider so
        // FallbackPolicy (require authenticated user) keeps working.
        _fallback = new DefaultAuthorizationPolicyProvider(options);
    }

    /// <summary>
    /// Returns a policy that requires the JWT to contain a "permission" claim
    /// whose value equals <paramref name="policyName"/>.
    /// Every non-null policy name is treated as a permission string — no
    /// whitelist or CSV lookup required.
    /// </summary>
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireClaim(CustomClaimTypes.Permission, policyName)
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    /// <inheritdoc/>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        _fallback.GetDefaultPolicyAsync();

    /// <inheritdoc/>
    /// Preserves the FallbackPolicy set in AddAuthorization (require authenticated
    /// user on every endpoint that carries no explicit [Authorize] attribute).
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        _fallback.GetFallbackPolicyAsync();
}

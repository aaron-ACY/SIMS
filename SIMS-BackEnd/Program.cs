using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using JsonWebTokenHandler = Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler;
using Microsoft.OpenApi.Models;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Domain.Constants;
using SIMS.Infrastructure;
using SIMS.Infrastructure.Settings;
using SIMS.Shared.Exceptions;
using SIMS.Shared.Models;
using SIMS_BackEnd.Authorization;
using SIMS_BackEnd.Constants;
using SIMS_BackEnd.Middleware;

// Disable the default JWT → .NET claim-type remapping so claim names like
// "sub" and "jti" are available as-is in HttpContext.User.Claims.
// Both handlers are cleared: .NET 8's JwtBearer uses JsonWebTokenHandler by
// default, so clearing only JwtSecurityTokenHandler would have no effect.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure + Application services ─────────────────────────── //
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.ContentRootPath);

// ── CORS ───────────────────────────────────────────────────────────── //
// Origins are read from Cors:AllowedOrigins in appsettings.
// Development: appsettings.Development.json lists localhost ports.
// Production: set Cors__AllowedOrigins__0, __1, ... environment variables
//             or override via appsettings.Production.json.
// An empty array blocks all cross-origin requests (safe default for prod).
const string CorsPolicyName = "SIMSCorsPolicy";

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();  
        }
        else
        {
            // No origins configured — deny all cross-origin requests.
            policy.SetIsOriginAllowed(_ => false);
        }
    });
});

// ── JWT Authentication ─────────────────────────────────────────────── //
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()!;

// Fail fast: secret must be supplied via User Secrets (dev) or Jwt__SecretKey
// env var (production). Never store it in appsettings.json.
if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
    throw new InvalidOperationException(
        "Jwt:SecretKey is not configured. " +
        "Development: run 'dotnet user-secrets set \"Jwt:SecretKey\" \"<key>\"' in SIMS-BackEnd/. " +
        "Production: set the Jwt__SecretKey environment variable.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Belt-and-braces: keep raw claim names ("sub", "jti", "role") on this
        // scheme regardless of the static maps above.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings.Issuer,
            ValidAudience            = jwtSettings.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero,
            // Tell the framework which claim carries the role so that
            // [Authorize(Roles = "...")] and User.IsInRole() work correctly.
            RoleClaimType = "role"
        };

        options.Events = new JwtBearerEvents
        {
            // After the signature + lifetime are valid, check our revocation list.
            OnTokenValidated = async context =>
            {
                var revokedTokenRepo = context.HttpContext.RequestServices
                    .GetRequiredService<IRevokedTokenRepository>();

                var jti = context.Principal?
                    .FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (jti is not null && await revokedTokenRepo.IsRevokedAsync(jti))
                {
                    context.Fail("Token has been revoked.");
                }
            },

            // 401 — missing, malformed, expired or revoked token.
            // Written by hand so the body matches the ApiResponse envelope
            // instead of the framework's empty WWW-Authenticate response.
            OnChallenge = context =>
            {
                context.HandleResponse();

                var errorCode = context.AuthenticateFailure is null
                    ? ErrorCode.UNAUTHENTICATED
                    : ErrorCode.INVALID_TOKEN;

                context.Response.StatusCode  = (int)errorCode.StatusCode;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsJsonAsync(ApiResponse.Fail(errorCode));
            },

            // 403 — authenticated but the role does not grant access.
            OnForbidden = context =>
            {
                context.Response.StatusCode  = (int)ErrorCode.UNAUTHORIZED.StatusCode;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsJsonAsync(
                    ApiResponse.Fail(ErrorCode.UNAUTHORIZED));
            }
        };
    });

// ── Rate limiting ─────────────────────────────────────────────────── //
// "auth" policy: 5 attempts per IP per minute on login/refresh.
// Uses a fixed window partitioned by remote IP so each client gets its own
// counter. Queue is set to 0 so excess requests are rejected immediately.
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit          = 5,
                Window               = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit           = 0   // reject immediately — no queuing
            }));

    // Return the same ApiResponse envelope as every other error in the app.
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode  = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            ApiResponse.Fail(ErrorCode.TOO_MANY_REQUESTS), cancellationToken);
    };
});

// ── Authorization ──────────────────────────────────────────────────── //
// PermissionPolicyProvider builds a policy on-demand for any permission
// string at request time, so permissions created via the API work
// immediately without a restart.  The foreach over Permissions.All is no
// longer needed — any [Authorize(Policy = "...")] is handled dynamically.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

builder.Services.AddAuthorization(options =>
{
    // Deny by default: any endpoint that declares no [Authorize] of its own
    // still requires an authenticated caller. Only the routes in
    // PublicEndpoints.All are exempted (see the convention below MapControllers).
    // This means forgetting an [Authorize] attribute yields a 401, not an
    // accidentally public endpoint.
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ── Global exception handling ──────────────────────────────────────── //
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();   // fallback required by UseExceptionHandler()

// ── Controllers ────────────────────────────────────────────────────── //
builder.Services.AddControllers();

// Replace the default ValidationProblemDetails body produced by [ApiController]
// with the same ApiResponse envelope every other endpoint returns.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors
                .Select(error => $"{entry.Key}: {error.ErrorMessage}"))
            .ToList();

        return new UnprocessableEntityObjectResult(
            ApiResponse.Fail(ErrorCode.VALIDATION_ERROR, errors));
    };
});

// ── Swagger / OpenAPI ──────────────────────────────────────────────── //
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "SIMS API",
        Version     = "v1",
        Description = "Student Information Management System"
    });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter your JWT token (without the 'Bearer ' prefix).",
        Reference    = new OpenApiReference
        {
            Id   = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

// ────────────────────────────────────────────────────────────────────── //
var app = builder.Build();

// ── Middleware pipeline ─────────────────────────────────────────────── //
// Must be first so it can catch anything thrown further down the pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicyName);
app.UseRateLimiter(); 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().Add(endpointBuilder =>
{
    if (endpointBuilder is not RouteEndpointBuilder routeBuilder)
        return;

    if (PublicEndpoints.IsPublic(routeBuilder.RoutePattern.RawText))
        endpointBuilder.Metadata.Add(new AllowAnonymousAttribute());
});

app.Run();

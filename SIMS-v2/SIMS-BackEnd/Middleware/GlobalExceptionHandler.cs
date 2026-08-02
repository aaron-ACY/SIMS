using Microsoft.AspNetCore.Diagnostics;
using SIMS.Shared.Exceptions;
using SIMS.Shared.Models;

namespace SIMS_BackEnd.Middleware;

/// <summary>
/// Central exception handler. Converts every unhandled exception into the
/// uniform <see cref="ApiResponse"/> envelope so clients never see a raw
/// stack trace or a ProblemDetails body.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (errorCode, message, errors) = Resolve(exception);

        if (errorCode.StatusCode is System.Net.HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception on {Path}", httpContext.Request.Path);
        else
            _logger.LogWarning("{Code} on {Path}: {Message}",
                errorCode.Code, httpContext.Request.Path, message);

        httpContext.Response.StatusCode  = (int)errorCode.StatusCode;
        httpContext.Response.ContentType = "application/json";

        var body = errors is null
            ? ApiResponse.Fail(errorCode, message)
            : ApiResponse.Fail(errorCode, errors, message);

        await httpContext.Response.WriteAsJsonAsync(body, cancellationToken);

        return true;
    }

    /// <summary>Maps an exception onto its error code, message and detail list.</summary>
    private static (ErrorCode Code, string Message, IEnumerable<string>? Errors) Resolve(
        Exception exception) => exception switch
    {
        // Known business error — surface its code and message as-is.
        AppException ex => (
            ex.ErrorCode,
            ex.Message,
            ex.Errors.Count > 0 ? ex.Errors : null),

        // Anything else is a bug: hide the details behind a generic message.
        _ => (
            ErrorCode.UNCATEGORIZED_EXCEPTION,
            ErrorCode.UNCATEGORIZED_EXCEPTION.Message,
            null)
    };
}

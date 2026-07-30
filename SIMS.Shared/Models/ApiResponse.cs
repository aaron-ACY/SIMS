using System.Text.Json.Serialization;
using SIMS.Shared.Exceptions;

namespace SIMS.Shared.Models;

/// <summary>
/// Uniform envelope returned by every API endpoint.
/// Optional members are omitted from the JSON payload when empty, so a plain
/// success response stays minimal: { "success": true, "result": { ... } }
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Result { get; init; }

    /// <summary>Business error code from <see cref="ErrorCode"/>; null on success.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Code { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    /// <summary>Field-level validation messages; omitted when empty.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Errors { get; init; }

    // ── Success ─────────────────────────────────────────────────────────── //

    public static ApiResponse<T> Ok(T result) =>
        new() { Success = true, Result = result };

    public static ApiResponse<T> Ok(T result, string? message) =>
        new() { Success = true, Result = result, Message = message };

    // ── Failure ─────────────────────────────────────────────────────────── //

    public static ApiResponse<T> Fail(ErrorCode errorCode, string? message = null) =>
        new()
        {
            Success = false,
            Code    = errorCode.Code,
            Message = message ?? errorCode.Message
        };

    public static ApiResponse<T> Fail(
        ErrorCode errorCode, IEnumerable<string> errors, string? message = null) =>
        new()
        {
            Success = false,
            Code    = errorCode.Code,
            Message = message ?? errorCode.Message,
            Errors  = errors.ToList().AsReadOnly()
        };
}

/// <summary>Non-generic variant for responses that carry no payload.</summary>
public class ApiResponse : ApiResponse<object?>
{
    public static ApiResponse Ok(string? message = null) =>
        new() { Success = true, Message = message };

    public new static ApiResponse Fail(ErrorCode errorCode, string? message = null) =>
        new()
        {
            Success = false,
            Code    = errorCode.Code,
            Message = message ?? errorCode.Message
        };

    public new static ApiResponse Fail(
        ErrorCode errorCode, IEnumerable<string> errors, string? message = null) =>
        new()
        {
            Success = false,
            Code    = errorCode.Code,
            Message = message ?? errorCode.Message,
            Errors  = errors.ToList().AsReadOnly()
        };
}

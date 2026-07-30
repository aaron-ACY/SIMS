namespace SIMS.Shared.Exceptions;

public class AppException : Exception
{
    public ErrorCode ErrorCode { get; }

    public IReadOnlyList<string> Errors { get; }

    public AppException(ErrorCode errorCode)
        : base(errorCode.Message)
    {
        ErrorCode = errorCode;
        Errors = [];
    }

    public AppException(ErrorCode errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
        Errors = [];
    }

    public AppException(ErrorCode errorCode, IEnumerable<string> errors)
        : base(errorCode.Message)
    {
        ErrorCode = errorCode;
        Errors = errors.ToList().AsReadOnly();
    }
}

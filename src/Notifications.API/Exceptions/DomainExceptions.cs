namespace MiniApi.Exceptions;

public class NotFoundException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
}

public class ValidationException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
}

// StatusCode y Detail varían: PRD-003 es 409, ORD-005 es 422, USR-004 es 403...
public class BusinessRuleException(string errorCode, string message, int statusCode, string detail)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;
    public int StatusCode { get; } = statusCode;
    public string Detail { get; } = detail;
}

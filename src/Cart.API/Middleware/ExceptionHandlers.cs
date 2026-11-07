using Microsoft.AspNetCore.Diagnostics;
using MiniApi.Models;

namespace MiniApi.Middleware;

public class NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not NotFoundException ex) return false;

        logger.LogWarning("Error de negocio {ErrorCode}: {ErrorMessage}", ex.ErrorCode, ex.Message);
        await ProblemWriter.WriteAsync(context, 404, "El recurso solicitado no fue encontrado.", ex.ErrorCode, ex.Message, ct);
        return true;
    }
}

public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        // BadHttpRequestException: JSON mal formado o tipos incorrectos en el body.
        var (code, message) = exception switch
        {
            ValidationException ex => (ex.ErrorCode, ex.Message),
            BadHttpRequestException => (ErrorCodes.InvalidData, ErrorCodes.InvalidDataMsg),
            _ => (null, null)
        };
        if (code is null) return false;

        logger.LogWarning("Error de validación {ErrorCode}: {ErrorMessage}", code, message);
        await ProblemWriter.WriteAsync(context, 400, "La solicitud contiene datos inválidos.", code, message!, ct);
        return true;
    }
}

public class BusinessRuleExceptionHandler(ILogger<BusinessRuleExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not BusinessRuleException ex) return false;

        logger.LogWarning("Error de negocio {ErrorCode}: {ErrorMessage}", ex.ErrorCode, ex.Message);
        await ProblemWriter.WriteAsync(context, ex.StatusCode, ex.Detail, ex.ErrorCode, ex.Message, ct);
        return true;
    }
}

// Red de seguridad: va registrado último.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IConfiguration config) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Error inesperado {ErrorCode}", ErrorCodes.Internal);

        // true solo en appsettings.Development.json. Nunca se expone el stack trace.
        var detail = config.GetValue<bool>("Errors:IncludeExceptionMessage")
            ? exception.Message
            : "Ocurrió un error inesperado.";

        await ProblemWriter.WriteAsync(context, 500, detail, ErrorCodes.Internal, ErrorCodes.InternalMsg, ct);
        return true;
    }
}

internal static class ProblemWriter
{
    public static Task WriteAsync(HttpContext context, int status, string detail,
        string errorCode, string errorMessage, CancellationToken ct)
    {
        var (type, title) = status switch
        {
            400 => ("https://tools.ietf.org/html/rfc7231#section-6.5.1", "Bad Request"),
            401 => ("https://tools.ietf.org/html/rfc7235#section-3.1", "Unauthorized"),
            403 => ("https://tools.ietf.org/html/rfc7231#section-6.5.3", "Forbidden"),
            404 => ("https://tools.ietf.org/html/rfc7231#section-6.5.4", "Not Found"),
            409 => ("https://tools.ietf.org/html/rfc7231#section-6.5.9", "Conflict"),
            422 => ("https://tools.ietf.org/html/rfc4918#section-11.2", "Unprocessable Entity"),
            _ => ("https://tools.ietf.org/html/rfc7231#section-6.6.1", "Internal Server Error")
        };

        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(new
        {
            type,
            title,
            status,
            detail,
            instance = context.Request.Path.Value,
            errorCode,
            errorMessage,
            correlationId = context.TraceIdentifier
        }, options: null, contentType: "application/problem+json", ct);
    }
}

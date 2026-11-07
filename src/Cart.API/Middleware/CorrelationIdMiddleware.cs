using System.Diagnostics;
using Serilog.Context;

namespace MiniApi.Middleware
{
    public class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        public const string HeaderName = "X-Correlation-Id";

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            context.TraceIdentifier = correlationId;

            // OnStarting y no asignación directa: UseExceptionHandler limpia los
            // headers antes de escribir el error, y se perdería el id.
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            var endpoint = $"{context.Request.Method} {context.Request.Path}";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("Endpoint", endpoint))
            {
                var watch = Stopwatch.StartNew();
                logger.LogInformation("Inicio de request {Endpoint}", endpoint);

                await next(context);

                logger.LogInformation("Fin de request {Endpoint} con status {StatusCode} en {ElapsedMs} ms",
                    endpoint, context.Response.StatusCode, watch.ElapsedMilliseconds);
            }
        }
    }
}
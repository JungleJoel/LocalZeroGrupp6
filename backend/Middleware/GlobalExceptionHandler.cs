using backend.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace backend.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            var (status, title) = exception switch
            {
                NotFoundException => (404, "Not found"),
                UnauthorizedException => (401, "Unauthorized"),
                ForbiddenException => (403, "Forbidden"),
                ConflictException => (409, "Conflict"),
                _ => LogUnhandled(exception)
            };

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var result = Results.Problem(
                title: title,
                detail: exception.Message,
                statusCode: status,
                extensions: new Dictionary<string, object?> { ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier }
            );

            await result.ExecuteAsync(context);

            return true;
        }

        private (int, string) LogUnhandled(Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
            return (500, "An unexpected error occurred");
        }
    }
}
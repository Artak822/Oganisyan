using System.Diagnostics;

namespace WishList.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        _logger.LogInformation(
            "Incoming HTTP {Method} request to {Path}",
            requestMethod,
            requestPath);

        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation(
            "HTTP {Method} request to {Path} completed with status {StatusCode} in {ElapsedMilliseconds}ms",
            requestMethod,
            requestPath,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}


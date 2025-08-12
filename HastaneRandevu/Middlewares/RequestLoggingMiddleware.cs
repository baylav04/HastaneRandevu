using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await _next(context);

        stopwatch.Stop();

        var method = context.Request.Method;
        var path = context.Request.Path;
        var statusCode = context.Response.StatusCode;
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
            method, path, statusCode, elapsedMs);
    }
}

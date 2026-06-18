using System.Diagnostics;

namespace PRN232.LMS.API.Middleware;

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
        
        var request = context.Request;
        var path = request.Path;
        var method = request.Method;

        _logger.LogInformation("HTTP Request Started: {Method} {Path}", method, path);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation("HTTP Request Completed: {Method} {Path} responded {StatusCode} in {ElapsedMs} ms", 
                method, path, statusCode, elapsedMs);
        }
    }
}

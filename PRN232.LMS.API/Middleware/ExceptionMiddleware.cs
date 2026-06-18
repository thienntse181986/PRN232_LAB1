using System.Net;
using System.Text.Json;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request execution.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var message = "Internal server error";
        object? errors = null;

        // In development mode, we can expose the actual exception details
        if (_env.IsDevelopment())
        {
            message = exception.Message;
            errors = exception.StackTrace;
        }

        var response = ApiResponse<object>.Fail(message, errors);
        
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);

        await context.Response.WriteAsync(jsonResponse);
    }
}

using System.Net;
using System.Text.Json;

namespace LibraryManagement.API.Extensions;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title, detail) = exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, "Bad Request", "A required parameter was not provided."),
            ArgumentException => (HttpStatusCode.BadRequest, "Bad Request", exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "Access denied."),
            NotImplementedException => (HttpStatusCode.NotImplemented, "Not Implemented", "This feature is not yet implemented."),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Request Timeout", "The request timed out."),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title,
            status = (int)statusCode,
            detail,
            instance = context.Request.Path.Value
        };

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
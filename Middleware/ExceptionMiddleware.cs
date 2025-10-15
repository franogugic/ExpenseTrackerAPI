using System.Net;
using System.Text.Json;

namespace ExpenceTrackerAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occured");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        HttpStatusCode status = exception switch
        {
            UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,
            KeyNotFoundException _ => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        var response = new
        {
            status = (int)status,
            message = exception.Message,
            traceId = context.TraceIdentifier
        };
        
        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
